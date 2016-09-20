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

    public class CodeGenerator
    {
        List<Instruction> code;
        List<Template.UpValInfo> upvalues;
        List<Template.LocalVarInfo> values;
        List<Value> literals;
        public CodeGenerator(int codesize)
        {
            code = new List<Instruction>(codesize);
            upvalues = new List<Template.UpValInfo>(codesize);
            values = new List<Template.LocalVarInfo>(codesize);
            literals = new List<Value>(codesize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="template"></param>
        /// <returns>return dummy value</returns>
        public bool Generate(AST ast)
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

        private bool GenerateListeral(AstLiteral ast)
        {
            Value value = ast.GetDatum();
            object refval = value.RefVal;
            if (refval is BoolClass)
            {
                code.Add(Instruction.MakeAB(OpCode.LOADBOOL, 0, value.AsBool() ? (ushort)1 : (ushort)0));
            }
            else if (refval is NumericalClass)
            {
                int kid = DefineLiteral(ref value);
                code.Add(Instruction.MakeAB(OpCode.LOADK, 0, (ushort)kid));
            }
            else
                throw new SystemException();

            return true;
        }

        private bool GenerateReference(AstReference ast)
        {
            Stx.Binding bind = ast.Binding;
            if (bind.IsPrimitive)
            {
                // this is the primitive such (+ a b)
            }
            else if (bind.IsGlobal)
            {

            }
            else
            {

            }
            return true;
        }

        private bool GenerateSet(AstSet ast)
        {
            return true;
        }

        private bool GenerateLambda(AstLambda ast)
        {

            return true;
        }

        private bool GenerateConditionIf(AstConditionIf ast)
        {
            Generate(ast.condExpression);
            Generate(ast.thenExperssion);
            Generate(ast.elseExpression);
            return true;
        }

        private bool GenerateCondition(AstCondition ast)
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
                    Generate(cond[1].AsAST());
                }
            }
            return true;
        }
        private bool GeneratePrimitive(AstPrimitive ast)
        {
            Symbol id = ast.Identifier.AsIdentifier();
            LinkedList<Value> args = ast.Arguments;
            return true;
        }

        private bool GenerateApplication(AstApplication ast)
        {
            return true;
        }

        private bool GenerateSequence(AstSequence ast)
        {
            LinkedList<Value> list = ast.BodyExpression;
            foreach (var val in list)
                Generate(val.AsAST());
            return true;
        }

        #region Temporary Variables

        public int maxTempVar;
        public int curTempVar;
        public int GetTempVar()
        {
            int num = curTempVar;
            maxTempVar = System.Math.Max(++curTempVar, maxTempVar);
            return num;
        }

        #endregion

        #region Literals Methods

        /// <summary>
        /// Create literal and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DefineLiteral(ref Value val)
        {
            int idx = ReferenceLiteral(ref val);
            if (idx >= 0) return idx;
            // literal is not found, define another one
            literals.Add(val);
            return literals.Count - 1;
        }

        /// <summary>
        /// Find literal and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int ReferenceLiteral(ref Value val)
        {
            for (var i = 0; i < literals.Count; i++)
                if (literals[i].Equals(val)) return i;
            return -1;
        }

        #endregion

        #region Varibales Methods

        /// <summary>
        /// Create local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DefineLocal(Symbol name)
        {
            int idx = ReferenceLocal(name);
            if (idx >= 0) return idx;
            // variable is not exists so we have to construct it
            Debug.Assert(values.Count < 256);
            Template.LocalVarInfo info = new Template.LocalVarInfo();
            info.Name = name;
            info.Index = (byte)values.Count;
            values.Add(info);
            return info.Index;
        }

        /// <summary>
        /// Find local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int ReferenceLocal(Symbol name)
        {
            for (var i = 0; i < values.Count; i++)
                if (values[i].Name == name) return i;
            return -1;
        }
        #endregion


    }


}