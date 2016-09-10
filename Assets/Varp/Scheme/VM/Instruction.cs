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

namespace VARP.Scheme.VM
{
    public struct Instruction
    {
        public enum OpCodes
        {
            SaveContinuation = 0, // 0
            FetchLiteral,         // 1
            Push,                 // 2 
            Apply,                // 3
            Bind,                 // 4
            MakeClosure,          // 5
            ToplevelGet,          // 6
            ToplevelSet,          // 7
            LocalGet,             // 8
            LocalSet,             // 9
            Return,               // 10
            End,                  // 11
            Jump,                 // 12
            JumpIfFalse,          // 13
            BindVarArgs,          // 14
        };

        public OpCodes OpCode;
        public ushort A;
        public ushort B;
        public uint AX;

        public Instruction(uint code)
        {
            // 6 bits for the opcode
            // 13 bits for A
            // 13 bits for B
            OpCode = (OpCodes)(code & 0x3F);
            A = (ushort)((code >> 6) & 0x1FFF);
            B = (ushort)(code >> 19);
            AX = code >> 6;
        }
    }
}