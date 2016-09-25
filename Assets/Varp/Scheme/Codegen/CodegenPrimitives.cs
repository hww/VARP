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
using System.Diagnostics;
using System.Collections.Generic;

namespace VARP.Scheme.Codegen
{
    using Data;
    using DataStructures;
    using VM;
    using Stx;

    public sealed partial class CodeGenerator
    {
        Symbol symADD = Symbol.Intern("+");
        Symbol symSUB = Symbol.Intern("-");
        Symbol symMUL = Symbol.Intern("*");
        Symbol symDIV = Symbol.Intern("/");
        Symbol symMOD = Symbol.Intern("%");
        Symbol symNEG = Symbol.Intern("neg");
        Symbol symNOT = Symbol.Intern("not");
        Symbol symAND = Symbol.Intern("and");
        Symbol symOR = Symbol.Intern("or");
        Symbol symLE = Symbol.Intern("<=");
        Symbol symLT = Symbol.Intern("<");
        Symbol symLEN = Symbol.Intern("length");
        Symbol symCONCAT = Symbol.Intern("concat");

        private int GeneratePrimitive(AstPrimitive ast)
        {
            Symbol sym = ast.Identifier.AsIdentifier();
            if (sym == symADD)
                return GenerateArith2(ast, OpCode.ADD);
            else if (sym == symSUB)
                return GenerateArith2(ast, OpCode.SUB);
            else if (sym == symMUL)
                return GenerateArith2(ast, OpCode.MUL);
            else if (sym == symMUL)
                return GenerateArith2(ast, OpCode.DIV);
            else if (sym == symMUL)
                return GenerateArith2(ast, OpCode.MOD);
            else if (sym == symNEG)
                return GenerateArith1(ast, OpCode.NEG);
            else if (sym == symNOT)
                return GenerateArith1(ast, OpCode.NOT);
            else if (sym == symAND)
                return GenerateAndOr(ast, OpCode.AND, true);
            else if (sym == symOR)
                return GenerateAndOr(ast, OpCode.OR, false);
            else if (sym == symLEN)
                return GenerateArith1(ast, OpCode.LEN);
            else if (sym == symCONCAT)
                return GenerateArithX(ast, OpCode.CONCAT);

            throw new SystemException();
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal int GenerateArith1(AstPrimitive ast, OpCode opcode)
        {
            ushort temp = TempIndex;

            LinkedList<Value> args = ast.Arguments;
            foreach (var arg in args)
            {
                Generate(arg.AsAST());
            }
            AddAB(opcode, temp, temp);
            return temp;
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal int GenerateArith2(AstPrimitive ast, OpCode opcode)
        {
            ushort temp = TempIndex;

            LinkedList<Value> args = ast.Arguments;
            foreach (var arg in args)
            {
                Generate(arg.AsAST());
            }
            AddABC(opcode, temp, temp, (ushort)(temp + 1));
            return temp;
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal int GenerateArithX(AstPrimitive ast, OpCode opcode)
        {
            // R(A) := R(B) .. ... .. R(C)

            ushort temp = TempIndex;

            LinkedList<Value> args = ast.Arguments;
            foreach (var arg in args)
                Generate(arg.AsAST());

            AddABC(OpCode.CONCAT, temp, temp, (ushort)(temp + args.Count));
            return temp;
        }


        /// <summary>
        /// This is the code generator for abstract opcode
        /// different with arithmetics is the first expression
        /// returns true will terminate execution
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal int GenerateAndOr(AstPrimitive ast, OpCode opcode, bool expects)
        {
            ushort temp = TempIndex;
            ushort exp = expects ? (ushort)1 : (ushort)0;
            LinkedList<Value> args = ast.Arguments;
            int[] jumps = new int[args.Count];
            int i = 0;
            foreach (var arg in args)
            {
                Generate(arg.AsAST());

                // if ((bool)R(A) != (bool)C) then {skip next instruction}
                AddABC(OpCode.TEST, temp, 0, exp);
                jumps[i++] = AddOpcode(Instruction.Nop);
            }
            // now make all jumps to the 
            int pc = PC;
            foreach (var jmp in jumps)
                Code[jmp] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp, pc));

            return temp;
        }
    }



}