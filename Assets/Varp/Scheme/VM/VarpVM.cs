/* 
 * Copyright (c) 2016 Valery Alex P.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace VARP.Scheme.VM
{
    using Data;
    using Exception;

    public sealed class VarpVM
    {
        public object RunTemplate(Template template)
        {
            return RunClosure(new Frame(null, template));
        }

        delegate ValueType RkDelegate(int i);

        private object RunClosure(Frame frame, params ValueType[] args)
        {
            Environment environment = frame.environment;
            Template template = frame.template;
            Value[] literals = template.Literals;
            Value[] values = frame.Values;
            Value[] upvalues = frame.Upvalues;

#if PROFILER
            _profiler.EnterFunction(null, TEMPLATE);
#endif
            try
            {
                while (true)
                {
                    Instruction op = frame.template.Code[frame.PC];
                    switch (op.OpCode)
                    {
                        case Instruction.OpCodes.MOVE:
                            values[op.A] = values[op.B];
                            break;

                        case Instruction.OpCodes.LOADK:
                            values[op.A] = template.Literals[op.B];
                            break;

                        case Instruction.OpCodes.LOADBOOL:
                            {
                                values[op.A].Set(op.B != 0);
                                if (op.C != 0) frame.PC++;
                            }
                            break;

                        case Instruction.OpCodes.LOADNIL:
                            {
                                int a = op.A;
                                int b = op.B;
                                do { values[a++].SetNil(); } while (b-- != 0);
                            }
                            break;

                        case Instruction.OpCodes.GETUPVAL:
                            ReadUpValue(frame, ref upvalues[op.B], out values[op.A]);
                            break;

                        case Instruction.OpCodes.GETGLOBAL:
                            break;

                        case Instruction.OpCodes.GETTABLE:
                            break;

                        case Instruction.OpCodes.SETGLOBAL:
                            break;

                        case Instruction.OpCodes.SETUPVAL:
                            WriteUpValue(frame, ref upvalues[op.B], ref values[op.A]);
                            break;

                        case Instruction.OpCodes.SETTABLE:
                            //{
                            //    int b = op.B;
                            //    var key = (b & Instruction.BitK) != 0 ?
                            //        literals[b & ~Instruction.BitK] :
                            //        values[b];
                            //
                            //    int c = op.C;
                            //    var value = (c & Instruction.BitK) != 0 ?
                            //        literals[c & ~Instruction.BitK] :
                            //        values[c];
                            //
                            //    //SetTable(variables[ op.A], ref key, ref value);
                            //}
                            break;

                        case Instruction.OpCodes.NEWTABLE:
                            {
                                int nArr = FbToInt(op.B);   // array size
                                int nNod = FbToInt(op.C);   // hash size
                                values[op.A].Set(new Table(nNod));
                            }
                            break;

                        case Instruction.OpCodes.SELF:
                            break;

                        case Instruction.OpCodes.ADD:
                        case Instruction.OpCodes.SUB:
                        case Instruction.OpCodes.MUL:
                        case Instruction.OpCodes.DIV:
                        case Instruction.OpCodes.MOD:
                        case Instruction.OpCodes.POW:
                            {
                                var ib = op.B;
                                var b = (ib & Instruction.BitK) != 0 ?
                                    literals[ib & ~Instruction.BitK] :
                                    values[ib];

                                var ic = op.C;
                                var c = (ic & Instruction.BitK) != 0 ?
                                    literals[ic & ~Instruction.BitK] :
                                    values[ic];

                                if (b.RefVal is INumeric &&
                                    c.RefVal is INumeric)
                                {
                                    double rv, bv = b.NumVal, cv = c.NumVal;
                                    switch (op.OpCode)
                                    {
                                        case Instruction.OpCodes.ADD: rv = bv + cv; break;
                                        case Instruction.OpCodes.SUB: rv = bv - cv; break;
                                        case Instruction.OpCodes.MUL: rv = bv * cv; break;
                                        case Instruction.OpCodes.DIV: rv = bv / cv; break;
                                        case Instruction.OpCodes.MOD: rv = bv % cv; break;
                                        case Instruction.OpCodes.POW: rv = Math.Pow(bv, cv); break;
                                        default: throw new NotImplementedException();
                                    }
                                    values[op.A].Set(rv);
                                }
                                else
                                {
                                    DoArith(op.OpCode, b, c, ref values[op.A]);
                                }
                            }
                            break;

                        case Instruction.OpCodes.NEG:
                            {
                                var ib = op.B;
                                var b = (ib & Instruction.BitK) != 0 ?
                                    literals[ib & ~Instruction.BitK] :
                                    values[ib];

                                if (b.RefVal is INumeric)
                                    values[op.A].Set(-b.NumVal);
                                else
                                    DoArith(op.OpCode, b, b, ref values[op.A]);
                            }
                            break;

                        case Instruction.OpCodes.NOT:
                            {
                                values[op.A].Set(!values[op.B].AsBool());
                            }
                            break;

                        case Instruction.OpCodes.LEN:
                            {
                                var ib = op.B;
                                var b = (ib & Instruction.BitK) != 0 ?
                                    literals[ib & ~Instruction.BitK] :
                                    values[ib];
                                DoGetLen(ref b, out values[op.A]);
                            }
                            break;

                        case Instruction.OpCodes.CONCAT:
                            {
                                int end = op.C + 1;
                                StringBuilder sb = new StringBuilder();
                                for (var n = op.B; n < end; n++)
                                    sb.Append(values[n].AsString());
                                values[op.A].Set(sb.ToString());
                            }
                            break;

                        case Instruction.OpCodes.JMP:
                            {
                                frame.PC += (int)op.BX; // SBX
                            }
                            break;

                        case Instruction.OpCodes.EQ:
                            {
                                ValueType rb = Rk(op.B);
                                ValueType rc = Rk(op.C);
                                //if ((rb == rc) )
                            }
                            break;

                        case Instruction.OpCodes.LT:
                            break;

                        case Instruction.OpCodes.LE:
                            break;

                        case Instruction.OpCodes.TEST:
                            break;

                        case Instruction.OpCodes.TESTSET:
                            break;

                        case Instruction.OpCodes.CALL:
                            break;

                        case Instruction.OpCodes.TAILCALL:
                            break;

                        case Instruction.OpCodes.RETURN:
                            frame = frame.parent;
                            break;

                        case Instruction.OpCodes.FORLOOP:
                            break;

                        case Instruction.OpCodes.FORPREP:
                            break;

                        case Instruction.OpCodes.TFORLOOP:
                            break;

                        case Instruction.OpCodes.SETLIST:
                            break;

                        case Instruction.OpCodes.CLOSE:
                            break;

                        case Instruction.OpCodes.CLOSURE:
                            break;

                        case Instruction.OpCodes.VARARG:
                            break;


                        default:
                            throw new InvalidOperationException("Invalid instruction");
                    }
   
                }
            }
            catch (SchemeError ex)
            {
                //return VALUE;
            }
            finally
            {
               // CONT = PREV_CONT;
               // ENVT = PREV_ENVT;
               // EVAL_STACK = PREV_EVAL_STACK;
               // TEMPLATE = PREV_TEMPLATE;
               // PC = PREV_PC;
            }


#if PROFILER
            _profiler.EnterFunction(null, TEMPLATE);
#endif
            return null;
        }

        /// <summary>
        /// Convert given value to number
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool ToNumber(Value val, ref double result)
        {
            if (val.RefVal is INumeric)
                return true;

            if (val.IsString)
            {
                result = Libs.StringLibs.GetNumerical(val.AsString());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Do arithmetic operation with conversion data to numerical types
        /// </summary>
        /// <param name="opCode">opce</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ret"></param>
        private void DoArith(Instruction.OpCodes opCode, Value a, Value b, ref Value ret)
        {
            double na = 0, nb = 0, rv = 0;

            if (ToNumber(a, ref na) && (opCode == Instruction.OpCodes.NEG || ToNumber(b, ref nb)))
            {

                switch (opCode)
                {
                    case Instruction.OpCodes.ADD: rv = na + nb; break;
                    case Instruction.OpCodes.SUB: rv = na - nb; break;
                    case Instruction.OpCodes.MUL: rv = na * nb; break;
                    case Instruction.OpCodes.DIV: rv = na / nb; break;
                    case Instruction.OpCodes.MOD: rv = na % nb; break;
                    case Instruction.OpCodes.POW: rv = Math.Pow(na, nb); break;

                    case Instruction.OpCodes.NEG: rv = -na; break;

                    default: Debug.Assert(false); rv = 0; break;
                }
                ret.Set(rv);
                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Read up-value
        /// </summary>
        /// <param name="frame">current frame</param>
        /// <param name="upVal">up value</param>
        /// <param name="ret">return value</param>
        private void ReadUpValue(Frame frame, ref Value upVal, out Value ret)
        {
            if (upVal.RefVal is Frame)
            {
                int varNum = (int)upVal.NumVal;
                ret = frame.Values[varNum];
                return;
            }
            if (upVal.RefVal is UpvalueClass)
            {
                // Get frame number and value number
                int number = (int)upVal.NumVal;
                ushort vindex = (ushort)number;  
                ushort vfridx = (ushort)(number >> 16);
                // Find requested frame
                while (vfridx>0)
                { frame = frame.parent; vfridx--; }
                // Update up-value reference
                upVal.RefVal = frame;
                upVal.NumVal = vindex;
                // Read upvalue
                ret = frame.Values[vindex];
                return;
            }
            throw new SystemException();
        }
        /// <summary>
        /// Write up-value
        /// </summary>
        /// <param name="frame">current frame</param>
        /// <param name="upVal">upvalue</param>
        /// <param name="value">value</param>
        private void WriteUpValue(Frame frame, ref Value upVal, ref Value value)
        {
            if (upVal.RefVal is Frame)
            {
                int varNum = (int)upVal.NumVal;
                (upVal.RefVal as Frame).Values[varNum] = value;
                return;
            }
            if (upVal.RefVal is UpvalueClass)
            {
                // Get frame number and value number
                int number = (int)upVal.NumVal;
                ushort vindex = (ushort)number;
                ushort vfridx = (ushort)(number >> 16);
                // Find requested frame
                while (vfridx > 0)
                { frame = frame.parent; vfridx--; }
                // Update up-value reference
                upVal.RefVal = frame;
                upVal.NumVal = vindex;
                // Write up-value
                (upVal.RefVal as Frame).Values[vindex] = value;
                return;
            }
            throw new SystemException();
        }

        private void DoGetLen(ref Value val, out Value ret)
        {
            var str = val.AsString();
            if (str != null)
            {
                ret.RefVal = FixnumClass.Instance;
                ret.NumVal = str.Length;
                return;
            }

            var asTable = val.RefVal as Table;
            if (asTable != null)
            {
                ret.RefVal = FixnumClass.Instance;
                ret.NumVal = asTable.Count;
                return;
            }

            throw new NotImplementedException();
        }

        internal static int FbToInt(int x)
        {
            int e = (x >> 3) & 0x1f;
            return e == 0 ? x : ((x & 7) + 8) << (e - 1);
        }

        #region Tables

        #endregion
    }
}