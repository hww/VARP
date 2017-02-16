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
    using REPL;
    using System.Diagnostics;

    public static class ArgumentsParser
    {

        #region Private methods

        /// <summary>
        /// Build argument pair from identifier and initializer only (lambda (:optional (x 1) (y 2) (z 3)) ...)
        /// </summary>
        private static void ParseArg(string name, Syntax stx, LinkedList<Value> list, AstEnvironment env, out Syntax id, out AST ast)
        {
            Debug.Assert(stx != null);
            Debug.Assert(list != null);
            Debug.Assert(env != null);
            var argc = list.Count;
            if (argc != 2) throw SchemeError.ArityError("let", "lambda: bad &key or &optional argument", 2, argc, list, stx);

            var a = list[0].AsSyntax();
            var b = list[1].AsSyntax();

            if (!a.IsIdentifier)
                SchemeError.ArgumentError(name, "symbol?", a);

            // compile initializer in the parent scope
            var exast = AstBuilder.Expand(b, env.Parent);
            id = a; ast = exast;
        }


        /// <summary>
        /// Build argument pair from identifier only (lambda (:optional x y z) ...)
        /// </summary>
        private static void ParseArg(string name, Syntax stx, Syntax identifier, AstEnvironment env, out Syntax var)
        {
            Debug.Assert(stx != null);
            Debug.Assert(identifier != null);
            Debug.Assert(env != null);
            if (!identifier.IsIdentifier)
                SchemeError.ArgumentError(name, "symbol?", identifier);
            var = identifier;
        }

        private static AstBinding ParseRequired(string name, Syntax stx, Syntax definition, AstEnvironment env, ArgumentBinding.Type type)
        {
            Syntax var = null;
            ParseArg(name, stx, definition, env, out var);
            var binding = new ArgumentBinding(env, var, type, null);
            env.Define(binding);
            return binding;
        }

        private static AstBinding ParseOptional(string name, Syntax stx, Syntax definition, AstEnvironment env, ArgumentBinding.Type type)
        {
            Syntax var = null;
            AST val = null;
            if (definition.IsIdentifier)
                ParseArg(name, stx, definition, env, out var);
            else if (definition.IsExpression)
                ParseArg(name, stx, definition.AsLinkedList<Value>(), env, out var, out val);
            else
                throw SchemeError.ArgumentError("lambda", "list?", definition);
            var binding = new ArgumentBinding(env, var, type, val);
            env.Define(binding);
            return binding;
        }

        private static AstBinding ParseBody(string name, Syntax stx, Syntax definition, AstEnvironment env)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Parser Methods

        public static AstEnvironment ParseLet(Syntax expression, LinkedList<Value> arguments, AstEnvironment environment)
        {
            var newenv = new AstEnvironment(environment);

            if (arguments == null)
                return newenv;

            foreach (var arg in arguments)
            {
                var argstx = arg.AsSyntax();
                if (argstx.IsExpression)
                    ParseOptional("let", argstx, argstx, newenv, ArgumentBinding.Type.Required);
                else
                    throw SchemeError.ArgumentError("let", "list?", argstx);
            }

            return newenv;
        }


        private delegate void AddDelegate(ref LinkedList<Value> first, ref LinkedList<Value> last, ValueClass obj);

        /// <summary>
        /// The result structure has lists of arguments where
        /// the variable names as syntaxes, but initializers as AST
        /// </summary>
        /// <param name="expression">expression where this aregumens located</param>
        /// <param name="arguments">the arguments list (syntax syntax syntax ...)</param>
        /// <param name="env">environment</param>
        /// <param name="args">destination arguments structure</param>
        public static AstEnvironment ParseLambda(Syntax expression, LinkedList<Value> arguments, AstEnvironment environment)
        {
            var newenv = new AstEnvironment(environment);

            if (arguments == null)
                return newenv;

            var arg_type = ArgumentBinding.Type.Required;

            /// ----------------------------------------------------------------------
            /// Waiting for the DSSSL keywords, when found change mode and return true
            /// ----------------------------------------------------------------------
            Func<Syntax, bool> SymbolToArgumentType = (Syntax stx) =>
            {
                if (!stx.IsSymbol) return false;

                var symbol = stx.GetDatum().AsSymbol();

                if (symbol == Symbol.OPTIONAL)
                    arg_type = ArgumentBinding.Type.Optionals;
                else if (symbol == Symbol.KEY)
                    arg_type = ArgumentBinding.Type.Key;
                else if (symbol == Symbol.REST)
                    arg_type = ArgumentBinding.Type.Rest;
                else if (symbol == Symbol.BODY)
                    arg_type = ArgumentBinding.Type.Body;
                else
                    return false;
                return true;
            };

            foreach (var arg in arguments)
            {
                var argstx = arg.AsSyntax();

                if (!SymbolToArgumentType(argstx))
                {
                    switch (arg_type)
                    {
                        case ArgumentBinding.Type.Required:
                            if (argstx.IsIdentifier)
                                ParseRequired("lambda", argstx, argstx, newenv, ArgumentBinding.Type.Required);
                            else
                                throw SchemeError.ArgumentError("lambda", "symbol?", argstx);
                            break;

                        case ArgumentBinding.Type.Optionals:
                            ParseOptional("lambda", argstx, argstx, newenv, ArgumentBinding.Type.Optionals);
                            break;

                        case ArgumentBinding.Type.Key:
                            ParseOptional("lambda", argstx, argstx, newenv, ArgumentBinding.Type.Key);
                            break;

                        case ArgumentBinding.Type.Rest:
                            ParseRequired("lambda", argstx, argstx, newenv, ArgumentBinding.Type.Rest);
                            arg_type = ArgumentBinding.Type.End;
                            break;

                        case ArgumentBinding.Type.Body:
                            ParseBody("lambda", argstx, argstx, newenv);
                            arg_type = ArgumentBinding.Type.End;
                            break;

                        case ArgumentBinding.Type.End:
                            throw SchemeError.SyntaxError("lambda", "unexpected extra argument", argstx);
                    }
                }
            }
            return newenv;
        }
        #endregion

    }

}