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
    using Exception;

    public sealed partial class CodeGenerator
    {

        public static Template GenerateCode(AST ast)
        {
            // create empty lambda function.
            // there are no any arguments
            var lambda = new CodeGenerator();

            // now generate the code
            var result = lambda.Generate(ast);

            lambda.GenerateReturn(result);

            // now lets create template from dummy lambda
            var templ = lambda.GetTemplate();

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
                return GenerateLiteral(ast as AstLiteral);

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

            throw SchemeError.ErrorWithName("codegen-generate", "unexpected ast", ast);
        }

        public int GenerateReturn(int argument)
        {
            // return R(A), ... ,R(A+B-2) (see note)
            if (argument < 0)
                AddAB(OpCode.RETURN, 0, 0);
            else
                AddAB(OpCode.RETURN, argument, 1);
            return argument;
        }

        public int GenerateResult(int argument)
        {
            // return R(A), ... ,R(A+B-2) (see note)
            if (argument < 0)
                AddAB(OpCode.RESULT, 0, 0);
            else
                AddAB(OpCode.RESULT, argument, 1);
            return argument;
        }

        /// <summary>
        /// Generate literal and return the position of literal in values
        /// list.
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        private int GenerateLiteral(AstLiteral ast)
        {
            var value = ast.isSyntaxLiteral ? ast.GetSyntax() : ast.GetDatum();
            var refval = value.RefVal;
            var sp = Push();
            if (refval == null)
            {
                Code.Add(Instruction.MakeAB(OpCode.LOADNIL, sp, 1));
                return sp;
            }
            else if(refval is BoolType)
            {
                Code.Add(Instruction.MakeAB(OpCode.LOADBOOL, sp, value.AsBool() ? (short)1 : (short)0));
                return sp;
            }
            else if (refval is Symbol)
            {
                if (refval == Symbol.NULL)
                {
                    Code.Add(Instruction.MakeAB(OpCode.LOADNIL, sp, 1));
                }
                else
                {
                    var kid = DefineLiteral(value);
                    Code.Add(Instruction.MakeABX(OpCode.LOADK, sp, kid));
                }
                return sp;
            }
            else
            {
                var kid = DefineLiteral(value);
                Code.Add(Instruction.MakeABX(OpCode.LOADK, sp, kid));
                return sp;
            }
        }

        private int GenerateReference(AstReference ast)
        {
            if (ast.IsLocal)
            {
                // Local case
                var dst = ReferenceLocal(ast.Identifier);
                return dst;
            }
            else if (ast.IsGlobal)
            {
                // R(A) := G[K(Bx)]
                var dst = Push();
                var name = ast.Identifier;
                var litIdx = DefineLiteral(new Value(name));
                var litIdxCur = Variables[ast.VarIdx].LitIdx;
                if (litIdxCur < 0)
                {
                    var var = Variables[ast.VarIdx];
                    var.LitIdx = litIdx;
                    Variables[ast.VarIdx] = var;
                }
                else if (litIdxCur != litIdx)
                    throw new System.Exception();

                AddABX(OpCode.GETGLOBAL, dst, ast.VarIdx);
                return dst;
            }
            else if (ast.IsUpValue)
            {
                var envIdx = (byte)ast.UpEnvIdx;
                var varIdx = (byte)ast.UpVarIdx;

                //if (envIdx > 0)
                //{
                // UpValue case
                // R(A) := U[B]
                var dst = Push();
                AddAB(OpCode.GETUPVAL, dst, 0);
                return dst;
                //}
                //else
                //{

                //}
            }
            else
                throw new System.Exception();
        }

        private short GenerateSet(AstSet ast)
        {
            var value = ast.Value;
            int target = Generate(value);


            if (ast.IsGlobal)
            {
                // G[K(Bx)] := R(A)
                var varIdx = new Value(ast.Identifier);
                var litIdx = DefineLiteral(varIdx);

                if (litIdx > Instruction.BxMask)
                    throw SchemeError.ErrorWithName("GenerateSet", "the index is too large for Bx", ast);

                var litIdxCur = Variables[ast.VarIdx].LitIdx;
                if (litIdxCur < 0)
                {
                    var var = Variables[ast.VarIdx];
                    var.LitIdx = litIdx;
                    Variables[ast.VarIdx] = var;
                }
                else if (litIdxCur != litIdx)
                    throw new System.Exception();

                AddABX(OpCode.SETGLOBAL, (short)target, ast.VarIdx);
            }
            else
            {
                var envIdx = ast.UpEnvIdx;
                var varIdx = ast.UpVarIdx;

                if (envIdx > 0)
                {
                    // UpValue case
                    // U[B] := R(A)
                    if (varIdx > Instruction.BMask)
                        throw SchemeError.ErrorWithName("GenerateSet", "the index is too large for RB", ast);
                    if (target > Instruction.AMask)
                        throw SchemeError.ErrorWithName("GenerateSet", "the index is too large for RA", ast);
                  //  AddAB(OpCode.SETUPVAL, (short)varIdx, (short)target);
                }
                else
                {
                    // Local case
                    // R(A) = R(B)
                    if (varIdx > Instruction.AMask)
                        throw SchemeError.ErrorWithName("GenerateSet", "the index is too large for RA", ast);
                    if (target > Instruction.BMask)
                        throw SchemeError.ErrorWithName("GenerateSet", "the index is too large for RB", ast);

                    AddAB(OpCode.MOVE, (short)varIdx, (short)target);
                }
            }
            return -1;
        }

        private int GenerateLambda(AstLambda ast)
        {
            // create empty lambda function.
            // there are no any arguments
            var lambda = new CodeGenerator(ast.ArgList.Length);

            // update list of arguments
            lambda.DefineArguments(ast.ArgList);

            /// R(A) := closure(KPROTO[Bx], R(A), ... , R(A + n))
            var temp = -1;

            // now generate the code, and get target register
            foreach (var v in ast.BodyExpression)
            {
                temp = lambda.Generate(v.AsAST());
                //if (tgt != temp)
                //    lambda.AddAB(OpCode.MOVE, temp, (byte)tgt);

            }
            lambda.GenerateReturn(temp);

            // now lets create template from dummy lambda
            var template = lambda.GetTemplate();
            // -----------------------------------------
            // now generate code for current function
            // -----------------------------------------
            temp = Push();
            var closureId = DefineLiteral(new Value(template));
            AddABX(OpCode.CLOSURE, temp, closureId);
            template.ResultIdx = temp;
            return temp;
        }

        /// <summary>
        /// Generate top level expression
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="ast"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public int GenerateTopLambda(Environment environment, AST ast)
        {
            if (!environment.IsLexical) throw new System.Exception();

            // create argument list
            var argList = environment.ToAstArray();

            // create empty lambda function.
            // there are no any arguments
            var lambda = new CodeGenerator(argList.Length);

            // update list of arguments
            lambda.DefineArguments(argList);

            /// R(A) := closure(KPROTO[Bx], R(A), ... , R(A + n))
            var temp = lambda.Generate(ast);

            // generate return code
            lambda.GenerateReturn(temp);

            // now lets create template from dummy lambda
            var template = lambda.GetTemplate();
            // -----------------------------------------
            // now generate code for current function
            // -----------------------------------------
            temp = Push();
            var closureId = DefineLiteral(new Value(template));
            AddABX(OpCode.CLOSURE, temp, closureId);
            template.ResultIdx = temp;

            return temp;
        }

        private int GenerateConditionIf(AstConditionIf ast)
        {
            var oldsp = SP;
            var temp = Generate(ast.condExpression);

            // if ((bool)R(A) != (bool)C) then {skip next instruction}
            AddABC(OpCode.TEST, temp, 0, 0);
            var else_jmp = AddOpcode(Instruction.Nop);

            // then 
            SP = oldsp;
            Generate(ast.thenExperssion);
            var then_end = AddOpcode(Instruction.Nop);

            SP = oldsp;
            var else_address = PC;
            Generate(ast.elseExpression);
            var else_end = PC;

            Code[else_jmp] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(else_jmp, else_address));
            Code[then_end] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(then_end, else_end));

            SP = temp;
            return temp;
        }

        private int GenerateCondition(AstCondition ast)
        {
            var temp = (byte)SP;

            if (ast.Conditions != null)
            {
                foreach (var v in ast.Conditions)
                {
                    var cond = v.AsLinkedList<Value>();
                    var c = cond[0];
                    if (cond.count == 1)
                    {
                        Generate(cond[0].AsAST());
                    }
                    else if (cond.count > 1)
                    {
                        Generate(cond[0].AsAST());

                        // if ((bool)R(A) != (bool)C) then {skip next instruction}
                        Code.Add(Instruction.MakeABC(OpCode.TEST, temp, 0, 0));
                        var jmp_address = PC; Code.Add(Instruction.Nop);

                        Generate(cond[1].AsAST());

                        var else_address = PC;
                        Code[jmp_address] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp_address, else_address));
                    }
                }
            }

            if (ast.ElseCase != null)
            {
                Generate(ast.ElseCase[1].AsAST());
            }

            SP = temp;
            return temp;
        }


        private int GenerateApplication(AstApplication ast)
        {
            // R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1))
            var temp = (short)(SP + 1);
            foreach (var val in ast.list)
            {
                var res = Generate(val.AsAST());
                if (res < SpMin)
                {
                    /// Case if it is addressed directly to the variable
                    /// MOVE R(A) := R(B)
                    AddAB(OpCode.MOVE, Push(), res);
                }
            }
            // now temp[] = [function, arg, arg, arg, ...]
            AddABC(OpCode.CALL, temp, (byte)(ast.list.Count - 1), 1);
            return temp;
        }

        private int GenerateSequence(AstSequence ast)
        {
            var temp = SP;
            var list = ast.BodyExpression;
            foreach (var val in list)
                temp = Generate(val.AsAST());
            return temp;
        }


    }
}