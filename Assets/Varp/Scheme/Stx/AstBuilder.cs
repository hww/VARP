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
using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using VM;
    using Stx;
    using VARP.Scheme.Tokenizing;


    public sealed class AstBuilder : SObject
    {
        [Flags]
        public enum Options
        {
            Default = 0,
            NoLambda = 1
        }

        #region Public Methods

        // Expand string @expression to abstract syntax tree, in given @env environment
        public static AST Expand(string expression, string filepath, Environment env)
        {
            var syntax = Parser.Parse(expression, filepath);
            return Expand(syntax, env);
        }

        // Expand string @syntax to abstract syntax tree, in global environment
        public static AST Expand(Syntax syntax, Environment env)
        {
            //if ((options & Options.NoLambda) == 0)
            {
                //   var list = new LinkedList<Value>();
                //   list.AddLast(new Value(Syntax.Lambda));
                //   list.AddLast(new Value(Syntax.Nil));
                //   list.AddLast(new Value(syntax));
                //
                //   var lambda = new Syntax(list, (Location)null);
                //
                //   return ExpandInternal(lambda, env);
            }
            //else
            {
                // expand expression
                var lexicalEnv = new Environment(env, Symbol.Intern("Lexical"));
                return ExpandInternal(syntax, lexicalEnv);
            }
        }

        // Expand string @syntax to abstract syntax tree, in given @env environment
        public static AST ExpandInternal(Syntax syntax, Environment env)
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
        public static AST ExpandLiteral(Syntax syntax, Environment env)
        {
            // n.b. value '() is null it will be as literal
            return new AstLiteral(syntax);
        }

        // aka: x
        public static AST ExpandIdentifier(Syntax syntax, Environment env)
        {
            if (!syntax.IsIdentifier) throw SchemeError.SyntaxError("ast-builder-expand-identifier", "expected identifier", syntax);

            var varname = syntax.GetDatum().AsSymbol();

            // Check and expand some of literals
            if (varname == Symbol.NULL)
            {
                return new AstLiteral(syntax);
            }

            // Find the variable in ast environment
            int envIdx = 0;
            var binding = env.LookupAstRecursively(varname, ref envIdx);

            if (binding == null)
            {
                // If variable is not found designate it as global variable
                var localIdx = env.Define(varname, new GlobalBinding(syntax));
                return new AstReference(syntax, AstReferenceType.Global, localIdx);
            }
            else
            {
                if (envIdx == 0)
                {
                    // local variable reference
                    return new AstReference(syntax, AstReferenceType.Local, binding.VarIdx, 0, 0);
                }
                else
                {
                    // up-value reference
                    if (binding is GlobalBinding || !binding.environment.IsLexical)
                    {
                        // global variable
                        var localIdx = env.Define(varname, new GlobalBinding(syntax));
                        return new AstReference(syntax, AstReferenceType.Global, localIdx);
                    }
                    else if (binding is LocalBinding || binding is ArgumentBinding)
                    {
                        // up value to local variable
                        var localIdx = env.Define(varname, new UpBinding(syntax, envIdx, binding.VarIdx));
                        return new AstReference(syntax, AstReferenceType.UpValue, localIdx, envIdx, binding.VarIdx);
                    }
                    else if (binding is UpBinding)
                    {
                        // upValue to other upValue
                        var upBinding = binding as UpBinding;
                        var nEnvIdx = upBinding.UpEnvIdx + envIdx;
                        var nVarIdx = upBinding.UpVarIdx;
                        var localIdx = env.Define(varname, new UpBinding(syntax, nEnvIdx, nVarIdx));
                        return new AstReference(syntax, AstReferenceType.UpValue, localIdx, nEnvIdx, nVarIdx);
                    }
                    else
                    {
                        throw new SystemException();
                    }
                }
            }
        }

        // aka: (...)
        public static AST ExpandExpression(Syntax syntax, Environment env)
        {
            var list = syntax.AsValueLinkedList();
            if (list == null) return new AstApplication(syntax, null);
            var ident = list[0].AsSyntax();
            if (ident.IsIdentifier)
            {
                var binding = env.LockupAstRecursively(ident.AsIdentifier());
                if (binding != null)
                {
                    if (binding is PrimitiveBinding)
                        return (binding as PrimitiveBinding).Primitive(syntax, env);
                }
            }
            // we do not find primitive. expand all expression with keyword at firs element
            return new AstApplication(syntax, ExpandListElements(list, 0, env));
        }

        // Expand list of syntax objects as: (#<syntax> #<syntax> ...)
        // aka: (...)
        public static LinkedList<Value> ExpandListElements(LinkedList<Value> list, int index, Environment env)
        {
            if (list == null) return null;

            var result = new LinkedList<Value>();

            foreach (var v in list)
            {
                if (index == 0)
                    result.AddLast(new Value( ExpandInternal(v.AsSyntax(), env)));
                else
                    index--;
            }

            return result;
        }

        #endregion
    }


}