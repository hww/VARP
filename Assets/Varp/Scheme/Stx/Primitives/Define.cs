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

namespace VARP.Scheme.Stx.Primitives
{
    using Exception;
    using REPL;
    using Data;
    using DataStructures;

    public sealed class PrimitiveDefine : BasePrimitive
    {
        // (define x ...)
        // (define (x) ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            ValueList list = stx.AsValueList();
            int argc = GetArgsCount(list);
            AssertArgsMinimum("define", "arity mismatch", 2, argc, list, stx);

            Syntax def_stx = list[0].AsSyntax();        // define
            Syntax var_stx = list[1].AsSyntax();        // ()

            if (var_stx.IsIdentifier)
            {
                AssertArgsMaximum("define", "arity mismatch", 2, argc, list, stx);
                Syntax val_stx = list[2].AsSyntax();

                // ----------------------------------------------------------------
                // identifier aka: (define x ...)
                // ----------------------------------------------------------------
                AST value = Expand(val_stx, env);

                Symbol var_id = var_stx.AsIdentifier();
                Binding bind = env.Lookup(var_id);
                if (bind == null) env.DefineVariable(var_id);

                return new AstDefine(stx, def_stx, var_stx, value, bind);
            }
            else if (var_stx.IsExpression)
            {
                // ----------------------------------------------------------------
                // identifier aka: (define (x ...) ...) as result lambda expression
                // ----------------------------------------------------------------
                ValueList args_list = var_stx.AsValueList();

                Arguments arguments = new Arguments();
                ArgumentsList.Parse(stx, args_list, env, ref arguments);

                ValueList lambda_body = AstBuilder.ExpandListElements(list, 2, env);
                AstLambda lambda = new AstLambda(stx, def_stx, arguments, lambda_body);

                Syntax identifier_stx = args_list[0].AsSyntax();
                Symbol identifier = identifier_stx.AsIdentifier();
                Binding binding = env.Lookup(identifier);
                if (binding == null) env.DefineVariable(identifier);

                return new AstDefine(stx, def_stx, identifier_stx, lambda, binding);
            }
            else
                throw SchemeError.ArgumentError("define", "symbol? or list?", var_stx);
        }
    }
}