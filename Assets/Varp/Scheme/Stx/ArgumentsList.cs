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
    using Data;
    using Exception;
    using REPL;

    public sealed class Arguments
    {
        public Pair required;
        public Pair optional;
        public Pair key;
        public Pair rest;
        public Pair body;
        public Pair values; // for let
    }
    public sealed class ArgumentsList
    {
        public enum Type
        {
            Required,       // (lambda (x y z) ...)
            Optionals,      // (lambda (x y #!optional z) ...)
            Key,            // (lambda (x y #!key z) ...)
            Rest,           // (lambda (x y #!rest z) ...)
            End             // after #!res value
        }



        delegate void AddDelegate(ref Pair first, ref Pair last, SObject obj);

        // @arguments is the list of syntax objects: (syntax syntax syntax ...)
        // result is the list: ((required) (optional) (key) (rest))
        public static void Parse(Pair arguments, LexicalEnvironment env, ref Arguments args)
        {
            if (arguments == null) return ;

            Pair curent = arguments;

            Pair required = null;
            Pair optional = null;
            Pair key = null;
            Pair rest = null;

            Pair required_last = null;
            Pair optional_last = null;
            Pair key_last = null;
            Pair rest_last = null;

            ArgumentsList.Type arg_type = ArgumentsList.Type.Required;

            AddDelegate Add = (ref Pair first, ref Pair last, SObject obj) =>
            {
                if (last == null)
                {
                    first = last = new Pair(obj, null);
                    return;
                }

                last.Cdr = new Pair(obj, null);
                last = last.Cdr as Pair;
            };

            Func<Syntax, bool> SymbolToArgumentType = (Syntax stx) =>
            {
                if (!(stx.GetDatum() is Symbol)) return false;

                Symbol symbol = stx.GetDatum<Symbol>();

                if (symbol == Symbol.OPTIONAL)
                {
                    arg_type = Type.Optionals;
                    if (optional == null) optional = optional_last = new Pair(stx, null);
                }
                else if (symbol == Symbol.KEY)
                {
                    arg_type = Type.Key;
                    if (key == null) key = key_last = new Pair(stx, null);
                }
                else if (symbol == Symbol.REST)
                {
                    arg_type = Type.Rest;
                    if (rest == null) rest = rest_last = new Pair(stx, null);
                }
                else
                {
                    return false;
                }
                return true;
            };

            while (curent != null)
            {
                Syntax arg = curent.Car as Syntax;
                if (!SymbolToArgumentType(arg))
                {
                    switch (arg_type)
                    {
                        case Type.Required:
                            if (arg.IsSyntaxIdentifier)
                                Add(ref required, ref required_last, new AstLiteral(arg));
                            else
                                throw new ContractViolation(arg, "symbol?", Inspector.Inspect(arg), "lambda: bad required argument");
                            break;

                        case Type.Optionals:
                            if (arg.IsSyntaxExpression)
                                Add(ref optional, ref optional_last, MakeArgPair(arg, arg.GetList(), env));
                            else
                                throw new ContractViolation(arg, "list?", Inspector.Inspect(arg), "lambda: bad &optional argument");
                            break;

                        case Type.Key:
                            if (arg.IsSyntaxExpression)
                                Add(ref key, ref key_last, MakeArgPair( arg, arg.GetList(), env));
                            else
                                throw new ContractViolation(arg, "list?", Inspector.Inspect(arg), "lambda: bad &key argument");
                            break;

                        case Type.Rest:
                            if (arg.IsIdentifier)
                                Add(ref rest, ref rest_last, new AstLiteral(arg));
                            else
                                throw new ContractViolation(arg, "symbol?", Inspector.Inspect(arg), "lambda: bad argument after &rest");
                            arg_type = Type.End;
                            break;
                        case Type.End:
                            throw new SyntaxError(arg, "lambda: unexpected extra argument " + Inspector.Inspect(arg));
                    }
                }
                    if (curent.Cdr == null) break;
                    if (curent.Cdr is Data.Pair)
                    {
                        curent = curent.Cdr as Data.Pair;
                    }
                    else
                    {
                        if (true)
                        {
                            throw new SyntaxError(arg, "lambda: unexpected dot syntax in arguments.  " + Inspector.Inspect(arg));
                        }
                        else
                        {
                            if (rest == null)
                            {
                                rest = Pair.ListFromArguments(new AstLiteral(curent.Cdr as Syntax), null);
                            }
                            else
                                throw new ContractViolation(curent.Cdr, "symbol?", Inspector.Inspect(curent.Cdr), "lambda: bad argument after '.'");
                            curent = null;
                        }
                    }


            }
            args.required = required;
            args.optional = optional;
            args.key = key;
            args.rest = rest; 
        }

        public static void ParseLetList(Pair arguments, LexicalEnvironment env, ref Arguments args)
        {
            if (arguments == null)
            {
                return;
            }

            Pair curent = arguments;

            Pair vars = null;
            Pair vals = null;

            Pair vars_last = null;
            Pair vals_last = null;

            AddDelegate Add = (ref Pair first, ref Pair last, SObject obj) =>
            {
                if (last == null)
                {
                    first = last = new Pair(obj, null);
                    return;
                }

                last.Cdr = new Pair(obj, null);
                last = last.Cdr as Pair;
            };

            while (curent != null)
            {
                Syntax arg = curent.Car as Syntax;
                if (arg.IsSyntaxExpression)
                {
                    Pair arg_pair = MakeArgPair(arg, arg.GetList(), env);
                    Add(ref vars, ref vars_last, arg_pair[0]);
                    Add(ref vals, ref vals_last, arg_pair[1]);
                }
                else
                    throw new ContractViolation(arg, "list?", Inspector.Inspect(arg), "let: bad argument definition");

                if (curent.Cdr == null) break;
                if (curent.Cdr is Data.Pair)
                {
                    curent = curent.Cdr as Data.Pair;
                }
                else
                {
                    throw new SyntaxError(arg, "lambda: unexpected dot syntax in arguments.  " + Inspector.Inspect(arg));
                }
            }
            args.required = vars;
            args.values = vals;
        }

        /// <summary>
        /// Build argument pair
        /// (identifier AST)
        /// </summary>
        static Pair MakeArgPair(Syntax stx, Pair list, LexicalEnvironment env)
        {
            int argc = Data.Pair.Length(list);
            if (argc != 2) new ArityMissmach(stx, 2, argc, "lambda: bad &key or &optional argument");

            Syntax a = list[0] as Syntax;
            Syntax b = list[1] as Syntax;

            if (!a.IsSyntaxIdentifier)
                new ContractViolation("symbol?", Inspector.Inspect(a.GetDatum()), "lambda: bad argument identifier");

            Pair pair = Pair.ListFromArguments(a, AstBuilder.Expand(b, env));

            return pair;
        }
    }
}