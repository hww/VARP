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

using System.Text;

namespace VARP.Scheme.VM
{
    public struct Instruction
    {
        public enum OpCodes
        {
            MOVE,       //<    A B      R(A) := R(B)
            LOADK,      //<    A Bx     R(A) := K(Bx)
            LOADBOOL,   //<    A B C    R(A) := (Bool)B; if (C) PC++
            LOADNIL,    //<    A B      R(A) := ... := R(B) := nil
            GETUPVAL,   //<    A B      R(A) := U[B]
            GETGLOBAL,  //<    A Bx     R(A) := G[K(Bx)]
            GETTABLE,   //<    A B C    R(A) := R(B)[RK(C)]
            SETGLOBAL,  //<    A Bx     G[K(Bx)] := R(A)
            SETUPVAL,   //<    A B      U[B] := R(A)
            SETTABLE,   //<    A B C    R(A)[RK(B)] := RK(C)
            NEWTABLE,   //<    A B C    R(A) := {} (size = B,C)
            SELF,       //<    A B C    R(A+1) := R(B); R(A) := R(B)[RK(C)]
            ADD,        //<    A B C    R(A) := RK(B) + RK(C)
            SUB,        //<    A B C    R(A) := RK(B) - RK(C)
            MUL,        //<    A B C    R(A) := RK(B) * RK(C)
            DIV,        //<    A B C    R(A) := RK(B) / RK(C)
            MOD,        //<    A B C    R(A) := RK(B) % RK(C)
            POW,        //<    A B C    R(A) := RK(B) ^ RK(C)
            NEG,        //<    A B      R(A) := -R(B)
            NOT,        //<    A B      R(A) := not R(B)
            LEN,        //<    A B	    R(A) := length of R(B)	
            CONCAT,     //<    A B      C R(A) := R(B) .. ... .. R(C)
            JMP,        //<    sBx      PC += sBx
            EQ,         //<    A B C    if ((RK(B) == RK(C)) ~= A) then PC++
            LT,         //<    A B C    if ((RK(B) < RK(C)) ~= A) then PC++
            LE,         //<    A B C    if ((RK(B) <= RK(C)) ~= A) then PC++
            TEST,       //<    A B C    if not (R(A) <=> C) then PC++
            TESTSET,    //<    A B C	if (R(B) <=> C) then R(A) := R(B) else pc++
            CALL,       //<    A B C    R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
            TAILCALL,   //<    A B C    return R(A)(R(A+1), ... ,R(A+B-1))
            RETURN,     //<    A B      return R(A), ... ,R(A+B-2) (see note)
            FORLOOP,    //<    A sBx    R(A)+=R(A+2); if R(A) <?= R(A+1) then { pc+=sBx; R(A+3)=R(A) }
            FORPREP,    //<    A sBx	R(A)-=R(A+2); pc+=sBx		
            TFORLOOP,   //<    A C      R(A+3), ... ,R(A+2+C) := R(A)(R(A+1), R(A+2)); 
                        //              if R(A+3) ~= nil then R(A+2)=R(A+3) else pc++
            SETLIST,    //<    A Bx     R(A)[Bx-Bx%FPF+i] := R(A+i), 1 <= i <= Bx%FPF+1
            CLOSE,      //<    A        close stack variables up to R(A)
            CLOSURE,    //<    A Bx     R(A) := closure(KPROTO[Bx], R(A), ... ,R(A+n))
            VARARG      //<    A B	    R(A), R(A+1), ..., R(A+B-1) = varargs
        };

        internal const int BitK = 1 << 8;

        public OpCodes OpCode;
        public byte A;
        public ushort B;
        public ushort C;
        public uint BX;

        public Instruction(uint code)
        {
            // 6 bits for the opcode
            // 8 bits for A
            // 18 bits for BX
            //    9 bits for B
            //    9 bits for C
            OpCode = (OpCodes)(code & 0x3F);
            A = (byte)((code >> 6) & 0x1FFF);
            B = (ushort)((code >> 14) & 0x1FF);
            C = (ushort)((code >> 21) & 0x1FF);
            BX = code >> 6;
        }

        public override string ToString()
        {
            var ret = new StringBuilder();

            ret.Append(OpCode.ToString());

            switch (OpCode)
            {
                case OpCodes.MOVE:
                    ret.AppendFormat(": R({0}) = R({1})", A, B);
                    break;

                case OpCodes.LOADK:
                    ret.AppendFormat(": R({0}) = K({1})", A, BX);
                    break;

                case OpCodes.LOADBOOL:
                    ret.AppendFormat(": R({0}) = (bool){1}", A, B != 0);
                    if (C != 0)
                        ret.Append("; PC++");
                    break;

                case OpCodes.LOADNIL:
                    ret.AppendFormat(B == 0 ? ": R({0}) = nil" : ": R({0}..{1}) = nil", A, A + B);
                    break;

                case OpCodes.GETUPVAL:
                    ret.AppendFormat(": R({0}) = U({1})", A, B);
                    break;

                case OpCodes.GETGLOBAL:
                    ret.AppendFormat(": R({0}) = G[K({1})]", A, BX);
                    break;

                case OpCodes.GETTABLE:
                    ret.AppendFormat(": R{0} = R({1})[{2}]", A, B, Rk(C));
                    break;

                case OpCodes.SETGLOBAL:
                    ret.AppendFormat(": G[K{1}] = R({0})", A, BX);
                    break;

                case OpCodes.SETUPVAL:
                    ret.AppendFormat(": U({1}) = R({0})", A, B);
                    break;

                case OpCodes.SETTABLE:
                    ret.AppendFormat(": R({0})[{1}] = {2}", A, Rk(B), Rk(C));
                    break;

                case OpCodes.NEWTABLE:
                    ret.AppendFormat(": R({0}) = {} (arr={1}, hash={2})", A, B, C);
                    break;

                case OpCodes.SELF:
                    ret.AppendFormat(": R({0}) = R({1}), R({2}) = R({1})[{3}]", A + 1, B, A, Rk(C));
                    break;

                case OpCodes.ADD:
                case OpCodes.SUB:
                case OpCodes.MUL:
                case OpCodes.DIV:
                case OpCodes.MOD:
                case OpCodes.POW:
                    ret.AppendFormat(": R({0}) = {1} {3} {2}", A, Rk(B), Rk(C), GetArithOp(OpCode));
                    break;

                case OpCodes.NEG:
                    ret.AppendFormat(": R({0}) = - R({1})", A, B);
                    break;

                case OpCodes.NOT:
                    ret.AppendFormat(": R({0}) = not {1}", A, Rk(B));
                    break;

                case OpCodes.LEN:
                    break;

                case OpCodes.CONCAT:
                    ret.AppendFormat(": R({0}) = R({1} ... {2})", A, B, C);
                    break;

                case OpCodes.JMP:
                    ret.Append(":");
                    short SBX = (short)(BX);
                    if (SBX != 0)
                        ret.AppendFormat(" PC {1}= {0}", System.Math.Abs(SBX), SBX > 0 ? "+" : "-");
                    if (A != 0)
                        ret.AppendFormat(" CloseUpVals( R# >= {0} )", A + 1);
                    break;

                case OpCodes.EQ:
                case OpCodes.LT:
                case OpCodes.LE:
                    ret.AppendFormat(": if( {0} {2} {1} ) PC++", Rk(B), Rk(C), GetCmpOp(OpCode, A != 0));
                    break;

                case OpCodes.TEST:
                    ret.AppendFormat(": if( R{0} <=> {1} ) then R{2} = {0} else PC++", B, C, A);
                    break;

                case OpCodes.TESTSET:
                    break;

                case OpCodes.CALL:
                    if (C == 0)
                        ret.AppendFormat(": R{0}... =", A);
                    else if (C == 2)
                        ret.AppendFormat(": R{0} =", A);
                    else if (C > 2)
                        ret.AppendFormat(": R{0}..R{1} =", A, A + C - 2);

                    ret.AppendFormat(" R{0}", A);

                    if (B == 0)
                        ret.AppendFormat("( R{0}... )", A + 1);
                    else if (B == 1)
                        ret.Append("()");
                    else if (B == 2)
                        ret.AppendFormat("( R{0} )", A + 1);
                    else
                        ret.AppendFormat("( R{0}..R{1} )", A + 1, A + 1 + B - 2);
                    break;


                case OpCodes.TAILCALL:
                    ret.AppendFormat(": return R{0}", A);

                    if (B == 0)
                        ret.AppendFormat("( R{0}... )", A + 1);
                    else if (B == 1)
                        ret.Append("()");
                    else if (B == 2)
                        ret.AppendFormat("( R{0} )", A + 1);
                    else
                        ret.AppendFormat("( R{0}..R{1} )", A + 1, A + 1 + B - 2);
                    break;

                case OpCodes.RETURN:
                    if (B == 0)
                        ret.AppendFormat(": return R{0}...", A);
                    else if (B == 1)
                        ret.Append(": return");
                    else if (B == 2)
                        ret.AppendFormat(": return R{0}", A);
                    else
                        ret.AppendFormat(": return R{0}..R{1}", A, A + B - 1);
                    break;

                case OpCodes.FORLOOP:
                    {
                        short SBx = (short)BX;
                        ret.AppendFormat(": R({0}) += R({1}); if R({0}) <?= R({2}) then PC+= {3}", A, A + 2, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCodes.FORPREP:
                    break;

                case OpCodes.TFORLOOP:
                    {
                        short SBx = (short)BX;
                        ret.AppendFormat(": if(type(R({1})) == table) {{ R({0}) = R({1}), PC {3}= {2} }}", A, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCodes.SETLIST:
                    ret.AppendFormat(": R({0}) R({1})", A, B);
                    break;

                case OpCodes.CLOSE:
                    ret.AppendFormat(": close stack up to R({0})", A);
                    break;

                case OpCodes.CLOSURE:
                    ret.AppendFormat(": R({0}) = MakeClosure( P{1} )", A, BX);
                    break;

                case OpCodes.VARARG:
                    ret.AppendFormat(": R({0}) = MakeClosure( P{1} )", A, BX);
                    break;

                default:
                    ret.Append("BAD!!!!!");
                    break;
            }

            return ret.ToString();
        }

        private static string GetArithOp(OpCodes op)
        {
            switch (op)
            {
                case OpCodes.ADD: return "+";
                case OpCodes.SUB: return "-";
                case OpCodes.MUL: return "*";
                case OpCodes.DIV: return "/";
                case OpCodes.MOD: return "%";
                case OpCodes.POW: return "^";
                default: return string.Empty;
            }
        }

        private static string GetCmpOp(OpCodes op, bool reverse)
        {
            switch (op)
            {
                case OpCodes.EQ: return reverse ? "!=" : "==";
                case OpCodes.LT: return reverse ? ">=" : "<";
                case OpCodes.LE: return reverse ? ">" : "<=";
                default: return string.Empty;
            }
        }

        private static string Rk(int i)
        {
            if ((i & BitK) != 0)
                return string.Format("K({0})", (i & ~BitK));
            else
                return string.Format("R({0})", i);
        }
        private static string R(int i)
        {
            return string.Format("R({0})", i);
        }
    }
}