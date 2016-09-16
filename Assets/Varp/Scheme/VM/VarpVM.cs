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
     /*
namespace VARP.Scheme.VM
{
    using Data;
    using Exception;
    using System;
    using System.Diagnostics;
    using System.Text;

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
            ValueType[] variables = frame.variables;

            RkDelegate Rk = (int i) =>
            {
                if ((i & Instruction.BitK) != 0)
                    return template.Literals[i];
                else
                    return variables[i];
            };

#if PROFILER
            _profiler.EnterFunction(null, TEMPLATE);
#endif
            try
            {
                while (true)
                {
                    Instruction i = frame.template.Code[frame.PC];
                    switch (i.OpCode)
                    {
                        case Instruction.OpCodes.MOVE:
                            variables[i.A] = variables[i.B];
                            break;

                        case Instruction.OpCodes.LOADK:
                            variables[i.A] = template.Literals[i.B];
                            break;

                        case Instruction.OpCodes.LOADBOOL:
                            {
                                int a = i.A;
                                int b = i.B;
                                do { variables[a++] = null; } while (b-- != 0);
                            }
                            break;

                        case Instruction.OpCodes.LOADNIL:
                            frame.variables[i.A] = i.B != 0 ? SBool.True : SBool.False;
                            if (i.C != 0) frame.PC++;
                            break;

                        case Instruction.OpCodes.GETUPVAL:
                            variables[i.A] = frame.upvariables[i.B];
                            break;

                        case Instruction.OpCodes.GETGLOBAL:
                            break;

                        case Instruction.OpCodes.GETTABLE:
                            break;

                        case Instruction.OpCodes.SETGLOBAL:
                            break;

                        case Instruction.OpCodes.SETUPVAL:
                            frame.upvariables[i.B] = frame.upvariables[i.A];
                            break;

                        case Instruction.OpCodes.SETTABLE:
                            break;

                        case Instruction.OpCodes.NEWTABLE:
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
                                ISchemeNumeric a = variables[i.A] as ISchemeNumeric;
                                ISchemeNumeric b = variables[i.B] as ISchemeNumeric;
                                ISchemeNumeric c = variables[i.C] as ISchemeNumeric;

                                if (b != null && c != null)
                                {
                                    double rv, bv = b.DoubleVlaue, cv = c.DoubleVlaue;
                                    switch (i.OpCode)
                                    {
                                        case Instruction.OpCodes.ADD: rv = bv + cv; break;
                                        case Instruction.OpCodes.SUB: rv = bv - cv; break;
                                        case Instruction.OpCodes.MUL: rv = bv * cv; break;
                                        case Instruction.OpCodes.DIV: rv = bv / cv; break;
                                        case Instruction.OpCodes.MOD: rv = bv % cv; break;
                                        case Instruction.OpCodes.POW: rv = Math.Pow(bv, cv); break;
                                        default: throw new NotImplementedException();
                                    }

                                    if (a == null)
                                        a = new SDouble(rv);
                                    else
                                        a.DoubleVlaue = rv;
                                }
                            }
                            break;

                        case Instruction.OpCodes.NEG:
                            {
                                ISchemeNumeric a = variables[i.A] as ISchemeNumeric;
                                ISchemeNumeric b = variables[i.B] as ISchemeNumeric;
                                if (b != null)
                                {
                                    if (a == null)
                                        a = (b as ValueType).Clone() as ISchemeNumeric;
                                    a.DoubleVlaue = -b.DoubleVlaue;
                                }
                                else
                                    DoArith(Instruction.OpCodes.NEG, b, b, ref frame.variables[i.A]);
                            }
                            break;

                        case Instruction.OpCodes.NOT:
                            {
                                variables[i.A] = variables[i.B].AsBool() ? SBool.True : SBool.False;
                            }
                            break;

                        case Instruction.OpCodes.LEN:
                            {
                                ValueType b = variables[i.B];
                                if (b is SString)
                                {

                                }
                            }
                            break;

                        case Instruction.OpCodes.CONCAT:
                            {
                                int len = i.C + 1;
                                StringBuilder sb = new StringBuilder();
                                for (var n = i.B; n < len; n++)
                                    sb.Append(variables[n].AsString());
                                SString ss = variables[i.A] as SString;
                                if (ss == null) ss = new SString();
                                ss.Value = sb.ToString();
                                variables[i.A] = ss;
                            }
                            break;

                        case Instruction.OpCodes.JMP:
                            break;

                        case Instruction.OpCodes.EQ:
                            {
                                ValueType rb = Rk(i.B);
                                ValueType rc = Rk(i.C);
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
        private bool ToNumber(ValueType val, ref double result)
        {
            if (val is ISchemeNumeric)
            {
                result = (val as ISchemeNumeric).DoubleVlaue;
                return true;
            }

            if (val is SString)
            {
                result = Libs.StringLibs.GetNumerical((val as SString).AsString());
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
        private void DoArith(Instruction.OpCodes opCode, ValueType a, ValueType b, ref ValueType ret)
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

                if (ret is ISchemeNumeric)
                    (ret as ISchemeNumeric).DoubleVlaue = rv;
                else
                    ret = new SDouble(rv);

                return;
            }

            throw new NotImplementedException();
        }
    }
}*/