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
    using VARP.Scheme.VM;

    public sealed class PrimitiveDefine : BasePrimitive
    {
        // (define x ...)
        // (define (x) ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            var list = stx.AsLinkedList<Value>();
            var argc = GetArgsCount(list);
            AssertArgsMinimum("define", "arity mismatch", 2, argc, list, stx);

            var def_stx = list[0].AsSyntax();        // define
            var var_stx = list[1].AsSyntax();        // ()

            if (var_stx.IsIdentifier)
            {
                AssertArgsMaximum("define", "arity mismatch", 2, argc, list, stx);
                var val_stx = list[2].AsSyntax();

                // ----------------------------------------------------------------
                // identifier aka: (define x ...)
                // ----------------------------------------------------------------
                var value = AstBuilder.ExpandInternal(val_stx, env);
                var var_id = var_stx.AsIdentifier();
                var binding = env.LookupAst(var_id);

                if (binding == null)
                {
                    // Global variable
                    return new AstSet(stx, var_stx, value, -1, -1, -1);
                }
                else if (binding is UpBinding)
                {
                    // Up-value variable
                    var ubind = binding as UpBinding;
                    return new AstSet(stx, var_stx, value, binding.VarIdx, ubind.UpEnvIdx, ubind.UpVarIdx);
                }
                else
                {
                    // Local variable
                    return new AstSet(stx, var_stx, value, binding.VarIdx, 0, 0);
                }
            }
            else if (var_stx.IsExpression)
            {
                // ----------------------------------------------------------------
                // identifier aka: (define (x ...) ...) as result lambda expression
                // ----------------------------------------------------------------
                var args_list = var_stx.AsLinkedList<Value>();

                var newenv = ArgumentsParser.ParseLambda(stx, args_list, env);

                var lambda_body = AstBuilder.ExpandListElements(list, 2, newenv);
                var lambda = new AstLambda(stx, def_stx, newenv, lambda_body);

                var identifier_stx = args_list[0].AsSyntax();
                var identifier = identifier_stx.AsIdentifier();
                var binding = env.LookupAst(identifier);

                if (binding == null)
                {
                    /// Global variable
                    return new AstSet(stx, var_stx, lambda, -1, -1, -1);
                }
                else if (binding is UpBinding)
                {
                    /// Up-value variable
                    var ubind = binding as UpBinding;
                    return new AstSet(stx, var_stx, lambda, binding.VarIdx, ubind.UpEnvIdx, ubind.UpVarIdx);
                }
                else
                {
                    /// Local variable
                    return new AstSet(stx, var_stx, lambda, binding.VarIdx, 0, 0);
                }
            }
            else
                throw SchemeError.ArgumentError("define", "symbol? or list?", var_stx);
        }
    }
}