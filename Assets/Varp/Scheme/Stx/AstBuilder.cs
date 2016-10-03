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

using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;


    public sealed class AstBuilder : ValueClass
    {
        

        #region Public Methods
        // Expand string @expression to abstract syntax tree in global environment
        public static AST Expand(string expression, string filepath)
        {
            return Expand(expression, filepath, SystemEnvironemnt.environment);
        }

        // Expand string @expression to abstract syntax tree, in given @env environment
        public static AST Expand(string expression, string filepath, AstEnvironment env)
        {
            Syntax syntax = Parser.Parse(expression, filepath);
            return Expand(syntax, env);
        }

        // Expand string @syntax to abstract syntax tree, in global environment
        public static AST Expand(Syntax syntax)
        {
            return Expand(syntax, SystemEnvironemnt.environment);
        }

        // Expand string @syntax to abstract syntax tree, in given @env environment
        public static AST Expand(Syntax syntax, AstEnvironment env)
        {
            if (syntax == null)
                return null;
            else if (syntax.IsLiteral)
                return ExpandLiteral(syntax, env);
            else if (syntax.IsIdentifier)
                return ExpandIdentifier(syntax, env);
            else if (syntax.IsExpression)
                return ExpandExpression(syntax, env);
            else
                throw SchemeError.SyntaxError("ast-builder-expand", "expected literal, identifier or list expression", syntax);
        }
        #endregion

        #region Private Expand Methods

        // aka: 99
        public static AST ExpandLiteral(Syntax syntax, AstEnvironment env)
        {
            // n.b. value '() is null it will be as literal
            return new AstLiteral(syntax);
        }

        // aka: x
        public static AST ExpandIdentifier(Syntax syntax, AstEnvironment env)
        {
            if (!syntax.IsIdentifier) throw SchemeError.SyntaxError("ast-builder-expand-identifier", "expected identifier", syntax);

            Symbol varname = syntax.GetDatum().AsSymbol();

            if (varname == Symbol.NULL)
                return new AstLiteral(syntax);

            AstBinding binding = env.Lookup(varname);
            /// If variable is not found designate it as global variable
            if (binding == null)
                return new AstReference(syntax, 0, -1, -1);
            /// Check the case of up-value
            if (binding.EnvIdx != env.Index)
                binding = env.Define(new UpBinding(env, binding.Id, binding));
            if (binding.IsUpvalue)
            {
                UpBinding ubind = binding as UpBinding;
                return new AstReference(syntax, ubind.VarIdx, ubind.EnvOffset, ubind.RefVarIdx);
            }
            /// It is ordinary local variable reference
            return new AstReference(syntax, binding.VarIdx, 0, 0);
        }

        // aka: (...)
        public static AST ExpandExpression(Syntax syntax, AstEnvironment env)
        {
            LinkedList<Value> list = syntax.AsValueLinkedList();
            if (list == null) return new AstApplication(syntax, null);
            Syntax ident = list[0].AsSyntax();
            if (ident.IsIdentifier)
            {
                AstBinding binding = env.Lookup(ident.AsIdentifier());
                if (binding != null)
                {
                    if (binding.IsPrimitive)
                        return (binding as PrimitiveBinding).Primitive(syntax, env);
                }
            }
            // we do not find primitive. expand all expression with keyword at firs element
            return new AstApplication(syntax, ExpandListElements(list, 0, env));
        }

        // Expand list of syntax objects as: (#<syntax> #<syntax> ...)
        // aka: (...)
        public static LinkedList<Value> ExpandListElements(LinkedList<Value> list, int index, AstEnvironment env)
        {
            if (list == null) return null;

            LinkedList<Value> result = new LinkedList<Value>();

            foreach (var v in list)
            {
                if (index == 0)
                    result.AddLast(new Value( Expand(v.AsSyntax(), env)));
                else
                    index--;
            }

            return result;
        }

        #endregion
    }


}