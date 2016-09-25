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


namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using REPL;

    public abstract class BaseArguments
    {
        #region Datum Methods
        public abstract Value AsDatum();

        protected Value GetDatum(Value value)
        {
            if (value.IsAST)
                return value.AsAST().GetDatum();
            else if (value.IsSyntax)
                return value.AsSyntax().GetDatum();
            else if (value.IsValuePair)
            {
                ValuePair pair = value.AsValuePair();
                LinkedList<Value> newPair = ValueLinkedList.FromArguments(GetDatum(pair.Item1), GetDatum(pair.Item2));
                return new Value(newPair);
            }
            else
                return value;
        }

        protected LinkedList<Value> GetDatum(LinkedList<Value> list)
        {
            LinkedList<Value> newlist = new LinkedList<Value>();
            foreach (var v in list)
            {
                newlist.AddLast(GetDatum(v));
            }
            return newlist;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Build argument pair from identifier and initializer only (lambda (:optional (x 1) (y 2) (z 3)) ...)
        /// </summary>
        static protected Value MakeArgPair(string name, Syntax stx, LinkedList<Value> list, Environment env)
        {
            int argc = list.Count;
            if (argc != 2) throw SchemeError.ArityError("let", "lambda: bad &key or &optional argument", 2, argc, list, stx);

            Syntax a = list[0].AsSyntax();
            Syntax b = list[1].AsSyntax();

            if (!a.IsIdentifier)
                SchemeError.ArgumentError(name, "symbol?", a);

            return new Value(new ValuePair(a, AstBuilder.Expand(b, env)));
        }
        /// <summary>
        /// Build argument pair from identifier only (lambda (:optional x y z) ...)
        /// </summary>
        static protected Value MakeArgPair(string name, Syntax stx, Syntax identifier, Environment env)
        {
            if (!identifier.IsIdentifier)
                SchemeError.ArgumentError(name, "symbol?", identifier);

            return new Value(new ValuePair(identifier, new AstLiteral(Syntax.False)));
        }

        #endregion
    }

    public sealed class LetArguments : BaseArguments
    {
        public Syntax expression; //< the expression where this arguments
        // -------------------------------------------------------------
        //  Example: (let ((x 1) (y 2)) ...)
        // -------------------------------------------------------------
        public LinkedList<Value> required;   //< (v1 v2)

        public override Value AsDatum()
        {
            LinkedList<Value> result = new LinkedList<Value>();
            if (required != null)
            {
                foreach (var v in required)
                    result.AddLast(GetDatum(v.AsValuePair().Item1));
            }

            return new Value(result);
        }
        public Value AsDatumValues()
        {
            LinkedList<Value> result = new LinkedList<Value>();
            if (required != null)
            {
                foreach (var v in required)
                    result.AddLast(GetDatum(v.AsValuePair().Item2));
            }
            return new Value(result);
        }

        public static void Parse(Syntax expression, LinkedList<Value> arguments, Environment env, ref LetArguments args)
        {
            args.expression = expression;

            args.required = new LinkedList<Value>();

            if (arguments == null)
                return;

            foreach (var arg in arguments)
            {
                Syntax argstx = arg.AsSyntax();
                if (argstx.IsExpression)
                {
                    ValuePair arg_pair = MakeArgPair("let", argstx, argstx.AsLinkedList<Value>(), env).AsValuePair();
                    args.required.AddLast(new Value(arg_pair));
                }
                else
                    throw SchemeError.ArgumentError("let", "list?", argstx);
            }
        }
    }

    public sealed class LambdaArguments : BaseArguments
    {
        // -------------------------------------------------------------
        // Keywords for keeping debug information
        // -------------------------------------------------------------
        public Syntax expression;             //< the expression where this arguments
        private Syntax optionalKwd;           //< &optional
        private Syntax keyKwd;                //< &key
        private Syntax restKwd;               //< &rest
        private Syntax bodyKwd;               //< &body
                                              // -------------------------------------------------------------
                                              //  Example: (lambda (v1 v2 :optional o1 (o2 1) :key k1 (k2 2) :rest r)
                                              // -------------------------------------------------------------
        public LinkedList<Value> required;   //< (v1 v2)
        public LinkedList<Value> optional;   //< (:optional o1 (o2 1))    
        public LinkedList<Value> key;        //< (:key k1 (k2 2))    
        public Syntax restIdent;             //< (:rest r)
        public AST bodyAst;                  //< (:body b) TODO

        #region Datum Methods

        public override Value AsDatum()
        {
            LinkedList<Value> result = new LinkedList<Value>();
            if (required != null)
                result.Append(GetDatum(required));
            if (optionalKwd != null)
            {
                result.AddLast(optionalKwd.GetDatum());
                result.Append(GetDatum(optional));
            }
            if (keyKwd != null)
            {
                result.AddLast(keyKwd.GetDatum());
                result.Append(GetDatum(key));
            }
            if (restKwd != null)
            {
                result.AddLast(restKwd.GetDatum());
                result.AddLast(GetDatum(restIdent.GetDatum()));
            }
            return new Value(result);
        }

        #endregion

        #region PArser Methods

        public enum Type
        {
            Required,       // (lambda (x y z) ...)
            Optionals,      // (lambda (x y #!optional z) ...)
            Key,            // (lambda (x y #!key z) ...)
            Rest,           // (lambda (x y #!rest z) ...)
            Body,           // (lambda (x y #!body z) ...)
            End             // after #!res value
        }

        delegate void AddDelegate(ref LinkedList<Value> first, ref LinkedList<Value> last, ValueClass obj);

        /// <summary>
        /// The result structure has lists of arguments where
        /// the variable names as syntaxes, but initializers as AST
        /// </summary>
        /// <param name="expression">expression where this aregumens located</param>
        /// <param name="arguments">the arguments list (syntax syntax syntax ...)</param>
        /// <param name="env">environment</param>
        /// <param name="args">destination arguments structure</param>
        public static void Parse(Syntax expression, LinkedList<Value> arguments, Environment env, ref LambdaArguments args)
        {
            args.expression = expression;

            args.required = null;
            args.optional = null;
            args.key = null;

            if (arguments == null) return;

            Type arg_type = Type.Required;

            /// ----------------------------------------------------------------------
            /// Waiting for the DSSSL keywords, when found change mode and return true
            /// ----------------------------------------------------------------------
            Func<Syntax, bool> SymbolToArgumentType = (Syntax stx) =>
            {
                if (!stx.IsSymbol) return false;

                Symbol symbol = stx.GetDatum().AsSymbol();

                if (symbol == Symbol.OPTIONAL)
                    arg_type = Type.Optionals;
                else if (symbol == Symbol.KEY)
                    arg_type = Type.Key;
                else if (symbol == Symbol.REST)
                    arg_type = Type.Rest;
                else if (symbol == Symbol.BODY)
                    arg_type = Type.Body;
                else
                    return false;
                return true;
            };

            foreach (var arg in arguments)
            {
                Syntax argstx = arg.AsSyntax();

                if (!SymbolToArgumentType(argstx))
                {
                    switch (arg_type)
                    {
                        case Type.Required:
                            if (args.required == null) args.required = new LinkedList<Value>();
                            if (argstx.IsIdentifier)
                                args.required.AddLast(arg);
                            else
                                throw SchemeError.ArgumentError("lambda", "symbol?", argstx);
                            break;

                        case Type.Optionals:
                            if (argstx.IsIdentifier)
                                args.optional.AddLast(MakeArgPair("lambda", argstx, argstx, env));
                            else if (argstx.IsExpression)
                                args.optional.AddLast(MakeArgPair("lambda", argstx, argstx.AsLinkedList<Value>(), env));
                            else
                                throw SchemeError.ArgumentError("lambda", "list?", argstx);
                            break;

                        case Type.Key:
                            if (argstx.IsIdentifier)
                                args.key.AddLast(MakeArgPair("lambda", argstx, argstx, env));
                            else if (argstx.IsExpression)
                                args.key.AddLast(MakeArgPair("lambda", argstx, argstx.AsLinkedList<Value>(), env));
                            else
                                throw SchemeError.ArgumentError("lambda", "list?", argstx);
                            break;

                        case Type.Rest:
                            if (argstx.IsIdentifier)
                                args.restIdent = arg.AsSyntax();
                            else
                                throw SchemeError.ArgumentError("lambda", "symbol?", argstx);
                            arg_type = Type.End;
                            break;

                        case Type.Body:
                            args.bodyAst = AstBuilder.Expand(argstx, env);
                            arg_type = Type.End;
                            break;

                        case Type.End:
                            throw SchemeError.SyntaxError("lambda", "unexpected extra argument", argstx);
                    }
                }
                else
                {
                    switch (arg_type)
                    {
                        case Type.Optionals:
                            args.optionalKwd = argstx;
                            args.optional = new LinkedList<Value>();
                            break;
                        case Type.Key:
                            args.keyKwd = argstx;
                            args.key = new LinkedList<Value>();
                            break;
                        case Type.Rest:
                            args.restKwd = argstx;
                            break;
                        case Type.Body:
                            args.bodyKwd = argstx;
                            break;

                    }
                }
            }
        }
        #endregion

    }

}