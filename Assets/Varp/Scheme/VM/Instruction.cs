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

using System.Diagnostics;
using System.Text;

namespace VARP.Scheme.VM
{
    public enum OpCode
    {
        NOP,
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
        AND,        //<    A B C    R(A) := RK(B) and RK(C)
        OR,         //<    A B C    R(A) := RK(B) or RK(C)
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

    [System.Serializable]
    public  struct Instruction
    {
        public static Instruction Nop = Instruction.MakeA(OpCode.NOP,0);

        public uint PackedValue;

        #region Inctruction Fileds

        private const int OpCodeShift = 0;
        private const int OpCodeMask = 0x3F;

        public OpCode OpCode
        {
            get { return (OpCode)((PackedValue >> OpCodeShift) & OpCodeMask); }
            set { SetField((int)value, OpCodeShift, OpCodeMask); }
        }

        private const int AShift = 6;
        private const int AMask = 0xFF;

        public int A
        {
            get { return (int)(PackedValue >> AShift) & AMask; }
            set { SetField(value, AShift, AMask); }
        }

        private const int BShift = 23;
        private const int BMask = 0x1FF;

        public int B
        {
            get { return (int)(PackedValue >> BShift) & BMask; }
            set { SetField(value, BShift, BMask); }
        }

        private const int CShift = 14;
        private const int CMask = 0x1FF;

        public int C
        {
            get { return (int)(PackedValue >> CShift) & CMask; }
            set { SetField(value, CShift, CMask); }
        }

        private const int BxShift = 14;
        private const int BxMask = 0x3FFFF;

        public int Bx
        {
            get { return (int)(PackedValue >> BxShift) & BxMask; }
            set { SetField(value, BxShift, BxMask); }
        }

        private const int SBxMax = (BxMask >> 1);

        public int SBx
        {
            get { return Bx - SBxMax; }
            set { Bx = value + SBxMax; }
        }

        private const int AxShift = 6;
        private const int AxMask = 0x3FFFFFF;

        public int Ax
        {
            get { return (int)(PackedValue >> AxShift) & AxMask; }
            set { SetField(value, AxShift, AxMask); }
        }
        private void SetField(int value, int shift, int mask)
        {
            Debug.Assert((value & ~mask) == 0);
            PackedValue = (uint)(PackedValue & ~(mask << shift)) | (uint)(value << shift);
        }

        internal const int BitK = 1 << 8;
        internal const int FieldsPerFlush = 50;

        #endregion

        public Instruction(uint code)
        {
            PackedValue = code;
        }

        /// <summary>
        /// Make instruction of format A
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeA(OpCode code, ushort a)
        {
            Instruction inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeAB(OpCode code, ushort a, ushort b)
        {
            Instruction inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.B = b;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B,C
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeABC(OpCode code, ushort a, ushort b, ushort c)
        {
            Instruction inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.B = b;
            inst.C = c;
            return inst;
        }
        /// <summary>
        /// Make instruction of format A,BX
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeABX(OpCode code, ushort a, int bx)
        {
            Instruction inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.Bx = bx;
            return inst;
        }

        /// <summary>
        /// Make instruction of format A,B or SBX
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        public static Instruction MakeASBX(OpCode code, ushort a, int sbx)
        {
            Instruction inst = new Instruction();
            inst.OpCode = code;
            inst.A = a;
            inst.SBx = sbx;
            return inst;
        }


        public override string ToString()
        {
            var ret = new StringBuilder();

            ret.Append(OpCode.ToString());

            switch (OpCode)
            {
                case OpCode.MOVE:
                    ret.AppendFormat(": R({0}) = R({1})", A, B);
                    break;

                case OpCode.LOADK:
                    ret.AppendFormat(": R({0}) = K({1})", A, Bx);
                    break;

                case OpCode.LOADBOOL:
                    ret.AppendFormat(": R({0}) = (bool){1}", A, B != 0);
                    if (C != 0)
                        ret.Append("; PC++");
                    break;

                case OpCode.LOADNIL:
                    ret.AppendFormat(B == 0 ? ": R({0}) = nil" : ": R({0}..{1}) = nil", A, A + B);
                    break;

                case OpCode.GETUPVAL:
                    ret.AppendFormat(": R({0}) = U({1})", A, B);
                    break;

                case OpCode.GETGLOBAL:
                    ret.AppendFormat(": R({0}) = G[K({1})]", A, Bx);
                    break;

                case OpCode.GETTABLE:
                    ret.AppendFormat(": R{0} = R({1})[{2}]", A, B, Rk(C));
                    break;

                case OpCode.SETGLOBAL:
                    ret.AppendFormat(": G[K{1}] = R({0})", A, Bx);
                    break;

                case OpCode.SETUPVAL:
                    ret.AppendFormat(": U({1}) = R({0})", A, B);
                    break;

                case OpCode.SETTABLE:
                    ret.AppendFormat(": R({0})[{1}] = {2}", A, Rk(B), Rk(C));
                    break;

                case OpCode.NEWTABLE:
                    ret.AppendFormat(": R({0}) = {} (arr={1}, hash={2})", A, B, C);
                    break;

                case OpCode.SELF:
                    ret.AppendFormat(": R({0}) = R({1}), R({2}) = R({1})[{3}]", A + 1, B, A, Rk(C));
                    break;

                case OpCode.ADD:
                case OpCode.SUB:
                case OpCode.MUL:
                case OpCode.DIV:
                case OpCode.MOD:
                case OpCode.POW:
                    ret.AppendFormat(": R({0}) = {1} {3} {2}", A, Rk(B), Rk(C), GetArithOp(OpCode));
                    break;

                case OpCode.NEG:
                    ret.AppendFormat(": R({0}) = - R({1})", A, B);
                    break;

                case OpCode.NOT:
                    ret.AppendFormat(": R({0}) = not {1}", A, Rk(B));
                    break;

                case OpCode.LEN:
                    break;

                case OpCode.CONCAT:
                    ret.AppendFormat(": R({0}) = R({1} ... {2})", A, B, C);
                    break;

                case OpCode.JMP:
                    ret.Append(":");
                    if (SBx != 0)
                        ret.AppendFormat(" PC {1}= {0}", System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    if (A != 0)
                        ret.AppendFormat(" CloseUpVals( R# >= {0} )", A + 1);
                    break;

                case OpCode.EQ:
                case OpCode.LT:
                case OpCode.LE:
                    ret.AppendFormat(": if( {0} {2} {1} ) PC++", Rk(B), Rk(C), GetCmpOp(OpCode, A != 0));
                    break;

                case OpCode.TEST:
                    ret.AppendFormat(": if( R{0} <=> {1} ) then R{2} = {0} else PC++", B, C, A);
                    break;

                case OpCode.TESTSET:
                    break;

                case OpCode.CALL:
                    {
                        /// result
                        if (C == 0)
                            ret.AppendFormat(":");
                        else if (C == 1)
                            ret.AppendFormat(": R{0} =", A);
                        else
                            ret.AppendFormat(": R{0}..R{1} =", A, A + C - 1);
                        /// function
                        ret.AppendFormat(" R{0}", A);
                        /// arguments
                        if (B == 0)
                            ret.AppendFormat("()");
                        else if (B == 1)
                            ret.AppendFormat("(R{0})", A);
                        else
                            ret.AppendFormat("(R{0}..R{1})", A + 1, A + 1 + B - 1);
                    }
                    break;


                case OpCode.TAILCALL:
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

                case OpCode.RETURN:
                    if (B == 1)
                        ret.AppendFormat(": return R{0}", A);
                    else
                        ret.AppendFormat(": return R{0}..R{1}", A, A + B);
                    break;

                case OpCode.FORLOOP:
                    {
                        ret.AppendFormat(": R({0}) += R({1}); if R({0}) <?= R({2}) then PC+= {3}", A, A + 2, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCode.FORPREP:
                    break;

                case OpCode.TFORLOOP:
                    {
                        ret.AppendFormat(": if(type(R({1})) == table) {{ R({0}) = R({1}), PC {3}= {2} }}", A, A + 1, System.Math.Abs(SBx), SBx > 0 ? "+" : "-");
                    }
                    break;

                case OpCode.SETLIST:
                    ret.AppendFormat(": R({0}) R({1})", A, B);
                    break;

                case OpCode.CLOSE:
                    ret.AppendFormat(": close stack up to R({0})", A);
                    break;

                case OpCode.CLOSURE:
                    ret.AppendFormat(": R({0}) = MakeClosure( K{1} )", A, Bx);
                    break;

                case OpCode.VARARG:
                    ret.AppendFormat(": R({0}) = MakeClosure( P{1} )", A, Bx);
                    break;

                default:
                    ret.Append("BAD!!!!!");
                    break;
            }

            return ret.ToString();
        }

        private static string GetArithOp(OpCode op)
        {
            switch (op)
            {
                case OpCode.ADD: return "+";
                case OpCode.SUB: return "-";
                case OpCode.MUL: return "*";
                case OpCode.DIV: return "/";
                case OpCode.MOD: return "%";
                case OpCode.POW: return "^";
                default: return string.Empty;
            }
        }

        private static string GetCmpOp(OpCode op, bool reverse)
        {
            switch (op)
            {
                case OpCode.EQ: return reverse ? "!=" : "==";
                case OpCode.LT: return reverse ? ">=" : "<";
                case OpCode.LE: return reverse ? ">" : "<=";
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