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
    using DataStructures;
    using Data;

    public sealed class PrimitiveSet : BasePrimitive
    {
        // (set! x 10)
        // (set! x (+ 1 2))
        public static AST Expand(Syntax stx, AstEnvironment env)
        {
            LinkedList<Value> list = stx.AsLinkedList<Value>();
            int argc = GetArgsCount(list);
            AssertArgsEqual("set!", "arity mismatch", 2, argc, list, stx);

            Syntax set_kwd = list[0].AsSyntax();
            Syntax var_stx = list[1].AsSyntax();
            Syntax val_stx = list[2].AsSyntax();

            Symbol var_id = var_stx.AsIdentifier();
            AstBinding binding = env.Lookup(var_id); // TODO! Maybe error when it is not defined
            AST value = AstBuilder.Expand(val_stx, env);
            if (binding == null)
            {
                /// Global variable
                return new AstSet(stx, var_stx, value, -1, -1, -1);
            }

            if (binding.EnvIdx != env.Index)
                binding = env.Define(new UpBinding(env, binding.Id, binding));

            if (binding.IsUpvalue)
            {
                /// Up-value variable
                UpBinding ubind = binding as UpBinding;
                return new AstSet(stx, var_stx, value, binding.VarIdx, ubind.RefEnvIdx, ubind.RefVarIdx);
            }
            else
            {
                /// Local variable
                return new AstSet(stx, var_stx, value, binding.VarIdx, 0, 0);
            }
        }
    }
}