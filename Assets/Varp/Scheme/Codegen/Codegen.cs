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
    using VM;
    using Data;
    using DataStructures;
    using Stx;

    public sealed partial class CodeGenerator
    {

        public static Template GenerateCode(AST ast)
        {
            // create empty lambda function.
            // there are no any arguments
            CodeGenerator lambda = new CodeGenerator();

            // now generate the code
            int result = lambda.Generate(ast);

            // now lets create template from dummy lambda
            Template templ = lambda.GetTemplate();

            return templ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="template"></param>
        /// <returns>return dummy value</returns>
        private int Generate(AST ast)
        {
            if (ast is AstLiteral)
                return GenerateListeral(ast as AstLiteral);

            if (ast is AstReference)
                return GenerateReference(ast as AstReference);

            if (ast is AstSet)
                return GenerateSet(ast as AstSet);

            if (ast is AstLambda)
                return GenerateLambda(ast as AstLambda);

            if (ast is AstConditionIf)
                return GenerateConditionIf(ast as AstConditionIf);

            if (ast is AstCondition)
                return GenerateCondition(ast as AstCondition);

            if (ast is AstPrimitive)
                return GeneratePrimitive(ast as AstPrimitive);

            if (ast is AstApplication)
                return GenerateApplication(ast as AstApplication);

            if (ast is AstSequence)
                return GenerateSequence(ast as AstSequence);

            throw new SystemException();
        }

        private int GenerateListeral(AstLiteral ast)
        {
            Value value = ast.GetDatum();
            object refval = value.RefVal;
            if (refval is BoolClass)
            {
                Code.Add(Instruction.MakeAB(OpCode.LOADBOOL, (byte)TempIndex, value.AsBool() ? (ushort)1 : (ushort)0));
                TempIndex++;
            }
            else if (refval is NumericalClass)
            {
                int kid = DefineLiteral(value);
                Code.Add(Instruction.MakeABX(OpCode.LOADK, (byte)TempIndex, kid));
                TempIndex++;
            }
            else
                throw new SystemException();

            return 1;
        }

        private int GenerateReference(AstReference ast)
        {
            byte temp = (byte)TempIndex;

            if (ast.IsGlobal)
            {
                // R(A) := G[K(Bx)]
                int litId = DefineLiteral(new Value(ast.Identifier));
                AddABX(OpCode.GETGLOBAL, temp, litId);
                TempIndex++;
            }
            else
            {
                byte envIdx = (byte)ast.EnvIdx;
                byte varIdx = (byte)ast.VarIdx;
                if (envIdx > 0)
                {
                    // UpValue case
                    byte upId = DefineUpValue(ast.Identifier, envIdx, varIdx);
                }
                else
                {
                    // Local case
                    byte upId = DefineLocal(ast.Identifier);

                }
            }
            return temp;
        }

        private int GenerateSet(AstSet ast)
        {
            AST value = ast.Value;
            int target = Generate(value);

            if (ast.IsGlobal)
            {
                // G[K(Bx)] := R(A)
                Value varid = new Value(ast.Identifier);
                int litId = DefineLiteral(varid);
                AddABX(OpCode.SETGLOBAL, (ushort)target, litId);
            }
            else
            {
                int envIdx = ast.EnvIdx;
                int varIdx = ast.VarIdx;
                if (envIdx > 0)
                {
                    // UpValue case

                }
                else
                {
                    // Local case

                }
            }
            return -1;
        }

        private int GenerateLambda(AstLambda ast)
        {
            /// R(A) := closure(KPROTO[Bx], R(A), ... , R(A + n))
            byte temp = (byte)TempIndex;

            // create empty lambda function.
            // there are no any arguments
            CodeGenerator lambda = new CodeGenerator(ast.ArgList);

            // now generate the code, and get target register
            foreach (var v in ast.BodyExpression)
                lambda.Generate(v.AsAST());

            // now lets create template from dummy lambda
            Template template = lambda.GetTemplate();

            int closureId = DefineLiteral(new Value(template));
            AddABX(OpCode.CLOSURE, temp, closureId);
            TempIndex++;
            return 1;
        }

        private int GenerateConditionIf(AstConditionIf ast)
        {
            byte temp = (byte)TempIndex;
            Generate(ast.condExpression);

            // if ((bool)R(A) != (bool)C) then {skip next instruction}
            AddABC(OpCode.TEST, temp, 0, 0);
            int jmp_address = AddOpcode(Instruction.Nop);

            Generate(ast.thenExperssion);
            int else_address = PC;
            Generate(ast.elseExpression);

            Code[jmp_address] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp_address, else_address));
            return temp;
        }

        private int GenerateCondition(AstCondition ast)
        {
            byte temp = (byte)TempIndex;

            if (ast.Conditions != null)
            {
                foreach (var v in ast.Conditions)
                {
                    LinkedList<Value> cond = v.AsLinkedList<Value>();
                    Value c = cond[0];
                    if (cond.count == 1)
                    {
                        Generate(cond[0].AsAST());
                    }
                    else if (cond.count > 1)
                    {
                        Generate(cond[0].AsAST());

                        // if ((bool)R(A) != (bool)C) then {skip next instruction}
                        Code.Add(Instruction.MakeABC(OpCode.TEST, temp, 0, 0));
                        int jmp_address = PC; Code.Add(Instruction.Nop);

                        Generate(cond[1].AsAST());

                        int else_address = PC;
                        Code[jmp_address] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp_address, else_address));
                    }
                }
            }

            if (ast.ElseCase != null)
            {
                Generate(ast.ElseCase[1].AsAST());
            }
            return temp;
        }


        private int GenerateApplication(AstApplication ast)
        {
            // R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
            byte temp = (byte)TempIndex;
            foreach (var val in ast.list)
            {
                Generate(val.AsAST());
            }
            // now temp[] = [function, arg, arg, arg, ...]
            AddAB(OpCode.CALL, temp, (byte)(ast.list.Count - 1));
            TempIndex = temp;
            return 1;
        }

        private int GenerateSequence(AstSequence ast)
        {
            byte temp = (byte)TempIndex;
            LinkedList<Value> list = ast.BodyExpression;
            foreach (var val in list)
                Generate(val.AsAST());
            return temp;
        }

        




    }


}