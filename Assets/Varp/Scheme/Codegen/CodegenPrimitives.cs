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
        private delegate short PrimCodeDelegate(AstPrimitive ast, OpCode opcode);

        private struct PrimCodegenItem
        {
            public PrimCodeDelegate method;
            public OpCode opcode;
        }

        private Dictionary<Symbol, PrimCodegenItem> primitivesTable;

        private void DefinePrimitiveCodegen(string name, PrimCodeDelegate method, OpCode opcode)
        {
            primitivesTable[Symbol.Intern(name)] = new PrimCodegenItem() { method = method, opcode = opcode };
        }

        private short GeneratePrimitive(AstPrimitive ast)
        {
            if (primitivesTable == null)
            {
                primitivesTable = new Dictionary<Symbol, PrimCodegenItem>();

                DefinePrimitiveCodegen("+", GenerateArith2, OpCode.ADD);
                DefinePrimitiveCodegen("-", GenerateArith2, OpCode.SUB);
                DefinePrimitiveCodegen("*", GenerateArith2, OpCode.MUL);
                DefinePrimitiveCodegen("/", GenerateArith2, OpCode.DIV);
                DefinePrimitiveCodegen("%", GenerateArith2, OpCode.MOD);
                DefinePrimitiveCodegen("=", GenerateArith2, OpCode.EQ);
                DefinePrimitiveCodegen(">", GenerateArith2, OpCode.GT);
                DefinePrimitiveCodegen("<", GenerateArith2, OpCode.LT);
                DefinePrimitiveCodegen("!=", GenerateArith2, OpCode.NE);
                DefinePrimitiveCodegen(">=", GenerateArith2, OpCode.GE);
                DefinePrimitiveCodegen("<=", GenerateArith2, OpCode.LE);
                DefinePrimitiveCodegen("pow", GenerateArith2, OpCode.POW);
                DefinePrimitiveCodegen("and", GenerateAndOr, OpCode.AND);
                DefinePrimitiveCodegen("or", GenerateAndOr, OpCode.OR);
                DefinePrimitiveCodegen("neg", GenerateArith1, OpCode.NEG);
                DefinePrimitiveCodegen("not", GenerateArith1, OpCode.NOT);
                DefinePrimitiveCodegen("len", GenerateArith1, OpCode.LEN);
                DefinePrimitiveCodegen("concat", GenerateArithX, OpCode.CONCAT);
            }
            var sym = ast.Identifier.AsIdentifier();

            var prim = primitivesTable[sym];
            return prim.method(ast, prim.opcode);
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal short GenerateArith1(AstPrimitive ast, OpCode opcode)
        {
            var result = Push();
            var args = ast.Arguments;
            foreach (var arg in args)
            {
                var expres = Generate(arg.AsAST());
                AddAB(opcode, result, expres);
                Pop();
            }
            return result;
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal short GenerateArith2(AstPrimitive ast, OpCode opcode)
        {
            var temp = Push();

            var args = ast.Arguments;
            int arg0 = Generate(args[0].AsAST());
            int arg1 = Generate(args[1].AsAST());
            AddABC(opcode, temp, (byte)arg0, (byte)arg1);

            return temp;
        }

        /// <summary>
        /// This is the code generator for abstract opcode
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        internal short GenerateArithX(AstPrimitive ast, OpCode opcode)
        {
            // R(A) := R(B) .. ... .. R(C)

            var temp = (short)(SP + 1);

            var args = ast.Arguments;
            foreach (var arg in args)
            {
                var res = Generate(arg.AsAST());
                if (res < SpMin)
                {
                    /// Case if it is addressed directly to the variable
                    /// MOVE R(A) := R(B)
                    AddAB(OpCode.MOVE, Push(), res);
                }
            }
            AddABC(OpCode.CONCAT, temp, temp, (short)(temp + args.Count - 1));
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
        internal short GenerateAndOr(AstPrimitive ast, OpCode opcode)
        {
            var isOrOperation = opcode == OpCode.OR;
            var expected = isOrOperation ? (short)1 : (short)0;
            /// This is the arguments list
            var args = ast.Arguments;
            /// Here will be jump instruction position for each argument
            var jumps = new int[args.Count];
            /// Put result to this value
            var result = Push();
            for (var i=0; i<args.Count; i++)
            {
                var argpos = Generate(args[i].AsAST());
                /// =======================================================
                /// if (R(B).AsBool == (bool)C) 
                ///     {skip next instruction}
                /// else
                ///     R(A) = R(B)
                /// =======================================================
                AddABC(OpCode.TESTSET, result, argpos, expected);
                jumps[i] = AddOpcode(Instruction.Nop);
                Pop();
            }
            Code.Add(Instruction.MakeAB(OpCode.LOADBOOL, result, isOrOperation ? (short)0 : (short)1));

            /// now make all jumps to the 
            var pc = PC;
            foreach (var jmp in jumps)
                Code[jmp] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp, pc));

            return result;
        }
    }



}