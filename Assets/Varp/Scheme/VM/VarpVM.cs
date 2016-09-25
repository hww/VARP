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
                for (int pc = frame.PC; pc < template.Code.Length; frame.PC = ++pc)
                {
                    var op = template.Code[pc];
                    switch (op.OpCode)
                    {
                        case OpCode.NOP:
                            break;

                        case OpCode.MOVE:
                            values[op.A] = values[op.B];
                            break;

                        case OpCode.LOADK:
                            values[op.A] = literals[op.B];
                            break;

                        case OpCode.LOADBOOL:
                            {
                                values[op.A].Set(op.B != 0);
                                if (op.C != 0) pc++;
                            }
                            break;

                        case OpCode.LOADNIL:
                            {
                                int a = op.A;
                                int b = op.B;
                                do { values[a++].SetNil(); } while (b-- != 0);
                            }
                            break;

                        case OpCode.GETUPVAL:
                            ReadUpValue(frame, ref upvalues[op.B], out values[op.A]);
                            break;

                        case OpCode.GETGLOBAL:
                            {
                                int c = op.C;
                                var key = (c & Instruction.BitK) != 0 ?
                                    literals[c & ~Instruction.BitK] :
                                    values[c];

                                Value upVal;
                                ReadUpValue(frame, ref upvalues[op.B], out upVal);
                                Binding bind = environment.LookupRecursively(upVal.AsSymbol());
                                if (bind != null)
                                    values[op.A].Set(bind.value);
                                else
                                    throw SchemeError.Error("vm-get-gloabal", "undefined variable", upVal.AsSymbol());
                            }
                            break;

                        case OpCode.GETTABLE:
                            break;

                        case OpCode.SETGLOBAL:
                            {
                                int b = op.B;
                                var key = (b & Instruction.BitK) != 0 ?
                                    literals[b & ~Instruction.BitK] :
                                    values[b];

                                int c = op.C;
                                var value = (c & Instruction.BitK) != 0 ?
                                    literals[c & ~Instruction.BitK] :
                                    values[c];

                                Value upVal;
                                ReadUpValue(frame, ref upvalues[op.A], out upVal);

                                Binding bind = environment.LookupRecursively(upVal.AsSymbol());
                                if (bind != null)
                                    bind.value.Set(value);
                                else
                                    throw SchemeError.Error("vm-set-gloabal", "undefined variable", upVal.AsSymbol());
                            }
                            break;

                        case OpCode.SETUPVAL:
                            WriteUpValue(frame, ref upvalues[op.B], ref values[op.A]);
                            break;

                        case OpCode.SETTABLE:
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

                        case OpCode.NEWTABLE:
                            {
                                int nArr = FbToInt(op.B);   // array size
                                int nNod = FbToInt(op.C);   // hash size
                                values[op.A].Set(new Table(nNod));
                            }
                            break;

                        case OpCode.SELF:
                            {
                                // TODO!
                                var table = values[op.B];
                                values[op.A + 1] = table;

                                int c = op.C;
                                var key = (c & Instruction.BitK) != 0 ?
                                    literals[c & ~Instruction.BitK] :
                                    values[c];

                                //GetTable(table, ref key, stackBase + op.A);
                            }
                            break;

                        case OpCode.ADD:
                        case OpCode.SUB:
                        case OpCode.MUL:
                        case OpCode.DIV:
                        case OpCode.MOD:
                        case OpCode.POW:
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
                                        case OpCode.ADD: rv = bv + cv; break;
                                        case OpCode.SUB: rv = bv - cv; break;
                                        case OpCode.MUL: rv = bv * cv; break;
                                        case OpCode.DIV: rv = bv / cv; break;
                                        case OpCode.MOD: rv = bv % cv; break;
                                        case OpCode.POW: rv = Math.Pow(bv, cv); break;
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

                        case OpCode.NEG:
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

                        case OpCode.NOT:
                            {
                                values[op.A].Set(!values[op.B].AsBool());
                            }
                            break;

                        case OpCode.LEN:
                            {
                                var ib = op.B;
                                var b = (ib & Instruction.BitK) != 0 ?
                                    literals[ib & ~Instruction.BitK] :
                                    values[ib];
                                DoGetLen(ref b, out values[op.A]);
                            }
                            break;

                        case OpCode.CONCAT:
                            {
                                int end = op.C + 1;
                                StringBuilder sb = new StringBuilder();
                                for (var n = op.B; n < end; n++)
                                    sb.Append(values[n].AsString());
                                values[op.A].Set(sb.ToString());
                            }
                            break;

                        case OpCode.JMP:
                            {
                                pc += op.SBx; 
                            }
                            break;

                        case OpCode.EQ:
                        case OpCode.LT:
                        case OpCode.LE:
                            {
                                int b = op.B;
                                var bv = (b & Instruction.BitK) != 0 ?
                                    literals[b & ~Instruction.BitK] :
                                    values[b];

                                int c = op.C;
                                var cv = (c & Instruction.BitK) != 0 ?
                                    literals[c & ~Instruction.BitK] :
                                    values[c];

                                bool test;

                                switch (op.OpCode)
                                {
                                    case OpCode.EQ:
                                        test = bv.RefVal == cv.RefVal ?
                                            bv.RefVal is NumericalClass || bv.NumVal == cv.NumVal :
                                            Equal(ref bv, ref cv);
                                        break;

                                    case OpCode.LT:
                                        test = (bv.RefVal is NumericalClass && cv.RefVal is NumericalClass) ?
                                            bv.NumVal < cv.NumVal :
                                            Less(ref bv, ref cv);
                                        break;

                                    case OpCode.LE:
                                        test = (bv.RefVal is NumericalClass && cv.RefVal is NumericalClass) ?
                                            bv.NumVal <= cv.NumVal :
                                            LessEqual(ref bv, ref cv);
                                        break;

                                    default: Debug.Assert(false); test = false; break;
                                }

                                if (test != (op.A != 0))
                                {
                                    pc++;
                                }
                                else
                                {
                                    op = template.Code[++pc];
                                    Debug.Assert(op.OpCode == OpCode.JMP);
                                    goto case OpCode.JMP;
                                }
                            }
                            break;


                        case OpCode.TEST:
                            {
                                // if ((bool)R(A) != (bool)C) then {skip next instruction}
                                var a = values[op.A].RefVal;
                                var isfalse = a == null || a == BoolClass.False;

                                if ((op.C == 0) == isfalse)
                                {
                                    op = template.Code[++pc];
                                    Debug.Assert(op.OpCode == OpCode.JMP);
                                    goto case OpCode.JMP;
                                }
                                else
                                {
                                    pc++;
                                }
                            }
                            break;

                        case OpCode.TESTSET:
                            {
                                var b = values[op.B].RefVal;
                                var test = b == null || b == BoolClass.False;

                                if ((op.C != 0) == test)
                                {
                                    pc++;
                                }
                                else
                                {
                                    values[op.A] = values[op.B];

                                    op = template.Code[++pc];
                                    Debug.Assert(op.OpCode == OpCode.JMP);
                                    goto case OpCode.JMP;
                                }
                            }
                            break;

                        case OpCode.CALL:
                            {
                                // A - has the closure to call
                                // B - has quantity of the arguments
                                // C - has quantity of results

                                var func = values[op.A];
                                var closure = func.As<Frame>();

                                int numArgs = op.B;
                                int numRetVals = op.C;

                                pc++; //return to the next instruction

                                closure.BeforeCall(numArgs, numRetVals);

                            }
                            break;

                        case OpCode.TAILCALL:
                            break;

                        case OpCode.RETURN:
                            frame = frame.parent;
                            break;

                        case OpCode.FORLOOP:
                            break;

                        case OpCode.FORPREP:
                            break;

                        case OpCode.TFORLOOP:
                            break;

                        case OpCode.SETLIST:
                            break;

                        case OpCode.CLOSE:
                            break;

                        case OpCode.CLOSURE:
                            {
                                /// R(A) := closure(KPROTO[Bx], R(A), ... , R(A + n))
                                Template ntempl = literals[op.B].As<Template>();
                                Frame nfram = new Frame(frame, ntempl); 
                                values[op.A].RefVal = nfram;
                            }
                            break;

                        case OpCode.VARARG:
                            {
                                //// R(A), R(A+1), ..., R(A+B-1) = varargs
                                //int max = frame.ValuesCount;
                                //int idx = frame.VarArgsIndex + frame.VarArgsCount;
                                //int cnt = op.B; // argc
                                //while (cnt>0)
                                //{
                                //    values[destIdx + i].RefVal = null;
                                //}
                                //
                                //
                                //
                                //if (VarArgsCount + Va)
                                //int qunatity = op.A
                                //if (frame.argc)
                                //int srcIdx = call.VarArgsIndex;
                                //int destIdx = stackBase + op.A;
                                //
                                //int numVarArgs = call.StackBase - srcIdx;
                                //int numWanted = op.B - 1;
                                //
                                //if (numWanted == -1)
                                //{
                                //    numWanted = numVarArgs;
                                //    stackTop = destIdx + numWanted;
                                //}
                                //
                                //int numHad = numWanted < numVarArgs ? numWanted : numVarArgs;
                                //for (int i = 0; i < numHad; i++)
                                //    values[destIdx + i] = values[srcIdx + i];
                                //
                                //for (int i = numHad; i < numWanted; i++)
                                //    values[destIdx + i].RefVal = null;
                            }
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
        private void DoArith(OpCode opCode, Value a, Value b, ref Value ret)
        {
            double na = 0, nb = 0, rv = 0;

            if (ToNumber(a, ref na) && (opCode == OpCode.NEG || ToNumber(b, ref nb)))
            {

                switch (opCode)
                {
                    case OpCode.ADD: rv = na + nb; break;
                    case OpCode.SUB: rv = na - nb; break;
                    case OpCode.MUL: rv = na * nb; break;
                    case OpCode.DIV: rv = na / nb; break;
                    case OpCode.MOD: rv = na % nb; break;
                    case OpCode.POW: rv = Math.Pow(na, nb); break;

                    case OpCode.NEG: rv = -na; break;

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

        #region Comparison
        private bool Equal(ref Value a, ref Value b)
        {
            var str = a.RefVal as string;
            if (str != null)
                return str.Equals(b.RefVal as string);

            if (a.RefVal is BoolClass)
                return b.AsBool() == a.AsBool();

            return a.Equals(b);
        }

        private bool Less(ref Value a, ref Value b)
        {
            var asStrA = a.RefVal as string;
            var asStrB = b.RefVal as string;

            if (asStrA != null && asStrB != null)
                return asStrA.CompareTo(asStrB) < 0;

            throw new NotImplementedException();
        }

        private bool LessEqual(ref Value a, ref Value b)
        {
            var asStrA = a.RefVal as string;
            var asStrB = b.RefVal as string;

            if (asStrA != null && asStrB != null)
                return asStrA.CompareTo(asStrB) <= 0;

            throw new NotImplementedException();
        }
        #endregion

        internal static int FbToInt(int x)
        {
            int e = (x >> 3) & 0x1f;
            return e == 0 ? x : ((x & 7) + 8) << (e - 1);
        }

        #region Tables

        #endregion

        #region Call

   
        #endregion
    }
}