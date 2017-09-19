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
            int result = lambda.Generate(ast);

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
        private short Generate(AST ast)
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

            throw SchemeError.Error("codegen-generate", "unexpected ast", ast);
        }

        public short GenerateReturn(short argument)
        {
            // return R(A), ... ,R(A+B-2) (see note)
            if (argument < 0)
                AddAB(OpCode.RETURN, 0, 0);
            else
                AddAB(OpCode.RETURN, argument, 1);
            return argument;
        }

        /// <summary>
        /// Generate literal and return the position of literal in values
        /// list.
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        private short GenerateLiteral(AstLiteral ast)
        {
            var value = ast.GetDatum();
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

        private short GenerateReference(AstReference ast)
        {
            if (ast.IsGlobal)
            {
                // R(A) := G[K(Bx)]
                var dst = Push();
                var litId = DefineLiteral(new Value(ast.Identifier));
                AddABX(OpCode.GETGLOBAL, dst, litId);
                return dst;
            }
            else
            {
                var envIdx = (byte)ast.RefEnvIdx;
                var varIdx = (byte)ast.RefVarIdx;
                if (envIdx > 0)
                {
                    // UpValue case
                    // R(A) := U[B]
                    var dst = Push();
                    AddAB(OpCode.GETUPVAL, dst, 0);
                    return dst;
                }
                else
                {
                    // Local case
                    var dst = ReferenceLocal(ast.Identifier);
                    return dst;
                }
            }
        }

        private short GenerateSet(AstSet ast)
        {
            var value = ast.Value;
            int target = Generate(value);


            if (ast.IsGlobal)
            {
                // G[K(Bx)] := R(A)
                var varid = new Value(ast.Identifier);
                var litId = DefineLiteral(varid);
                if (litId > Instruction.BxMask)
                    throw SchemeError.CompillerError("GenerateSet", "the index is too large for Bx", ast);
                AddABX(OpCode.SETGLOBAL, (short)target, litId);
            }
            else
            {
                var envIdx = ast.RefEnvIdx;
                var varIdx = ast.RefVarIdx;

                if (envIdx > 0)
                {
                    // UpValue case
                    // U[B] := R(A)
                    if (varIdx > Instruction.BMask)
                        throw SchemeError.CompillerError("GenerateSet", "the index is too large for RB", ast);
                    if (target > Instruction.AMask)
                        throw SchemeError.CompillerError("GenerateSet", "the index is too large for RA", ast);
                  //  AddAB(OpCode.SETUPVAL, (short)varIdx, (short)target);
                }
                else
                {
                    // Local case
                    // R(A) = R(B)
                    if (varIdx > Instruction.AMask)
                        throw SchemeError.CompillerError("GenerateSet", "the index is too large for RA", ast);
                    if (target > Instruction.BMask)
                        throw SchemeError.CompillerError("GenerateSet", "the index is too large for RB", ast);

                    AddAB(OpCode.MOVE, (short)varIdx, (short)target);
                }
            }
            return -1;
        }

        private short GenerateLambda(AstLambda ast)
        {

            // create empty lambda function.
            // there are no any arguments
            var lambda = new CodeGenerator(ast.ArgList);

            /// R(A) := closure(KPROTO[Bx], R(A), ... , R(A + n))
            short temp = -1;

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
            return temp;
        }

        private short GenerateConditionIf(AstConditionIf ast)
        {
            var temp = (byte)SP;
            Generate(ast.condExpression);

            // if ((bool)R(A) != (bool)C) then {skip next instruction}
            AddABC(OpCode.TEST, temp, 0, 0);
            var jmp_address = AddOpcode(Instruction.Nop);

            Generate(ast.thenExperssion);
            var else_address = PC;
            Generate(ast.elseExpression);

            Code[jmp_address] = Instruction.MakeASBX(OpCode.JMP, 0, Jmp(jmp_address, else_address));
            return temp;
        }

        private short GenerateCondition(AstCondition ast)
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
            return temp;
        }


        private short GenerateApplication(AstApplication ast)
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

        private short GenerateSequence(AstSequence ast)
        {
            var temp = SP;
            var list = ast.BodyExpression;
            foreach (var val in list)
                temp = Generate(val.AsAST());
            return temp;
        }


    }
}