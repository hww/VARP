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

namespace VARP.Scheme.Stx
{
    using Data;
    using Exception;

    public class AstBuilder
    {
        public static SystemEnvironemnt environment = new SystemEnvironemnt();
        #region Public Methods
        // Expand string @expression to abstract syntax tree in global environment
        public static AST Expand(string expression, string filepath)
        {
            return Expand(expression, filepath, environment);
        }

        // Expand string @expression to abstract syntax tree, in given @env environment
        public static AST Expand(string expression, string filepath, LexicalEnvironment env)
        {
            Syntax syntax = Parser.Parse(expression, filepath);
            return Expand(syntax, env);
        }

        // Expand string @syntax to abstract syntax tree, in global environment
        public static AST Expand(Syntax syntax)
        {
            return Expand(syntax, environment);
        }

        // Expand string @syntax to abstract syntax tree, in given @env environment
        public static AST Expand(Syntax syntax, LexicalEnvironment env)
        {
            if (syntax == null)
                return null;
            else if (syntax.IsLiteral)
                return ExpandLiteral(syntax, env);
            else if (syntax.IsSymbol)
                return ExpandIdentifier(syntax, env);
            else if (syntax.IsList)
                return ExpandExpression(syntax, env);
            else
                throw new SyntaxError(syntax, "Expected literal, identifier or list expression");
        }
        #endregion

        #region Private Expand Methods

        // aka: 99
        public static AST ExpandLiteral(Syntax syntax, LexicalEnvironment env)
        {
            // n.b. value '() is null it will be as literal
            return new AstLiteral(syntax);
        }

        // aka: x
        public static AST ExpandIdentifier(Syntax syntax, LexicalEnvironment env)
        {
            if (!syntax.IsSymbol || syntax.IsLiteral) new SyntaxError(syntax, "Expected identifier");
            Symbol varname = syntax.GetDatum() as Symbol;
            LexicalBinding binding = env.Lookup(varname);
            if (binding == null)
                throw new SyntaxError(syntax, "Expected identifier");
            return new AstReference(syntax, binding);
        }

        // aka: (...)
        public static AST ExpandExpression(Syntax syntax, LexicalEnvironment env)
        {
            Pair list = syntax.GetList();
            if (list == null) return new AstApplication(syntax, null);
            Syntax ident = list.Car as Syntax;
            if (ident.IsSymbol)
            {
                LexicalBinding binding = env.Lookup(ident.expression as Symbol);
                if (binding != null)
                {
                    if (binding.IsPrimitive)
                        return binding.Primitive(syntax, env);
                }
            }
            return new AstApplication(syntax, ExpandListElements(list, env));
        }

        // Expand list of syntax objects as: (#<syntax> #<syntax> ...)
        // aka: (...)
        public static Pair ExpandListElements(Pair list, LexicalEnvironment env)
        {
            if (list == null) return null;

            Pair first = new Pair();
            Pair last = first;
            while (list != null)
            {
                last.Car = Expand(list.Car as Syntax, env);
                if (list.Cdr is Pair)
                {
                    last.Cdr = new Pair();
                    last = last.Cdr as Pair;
                    list = list.Cdr as Pair;
                }
                else
                {
                    last.Cdr = Expand(list.Cdr as Syntax, env);
                    last = null;
                    break;
                }
            }
            return first;
        }

        #endregion
    }


}