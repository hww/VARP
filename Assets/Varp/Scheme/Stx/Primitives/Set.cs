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

namespace VARP.Scheme.Stx.Primitives
{
    using DataStructures;
    using Data;
    using VM;

    public sealed class PrimitiveSet : BasePrimitive
    {
        // (set! x 10)
        // (set! x (+ 1 2))
        public static AST Expand(Syntax stx, Environment env)
        {
            var list = stx.AsLinkedList<Value>();
            var argc = GetArgsCount(list);
            AssertArgsEqual("set!", "arity mismatch", 2, argc, list, stx);

            var set_kwd = list[0].AsSyntax();
            var var_stx = list[1].AsSyntax();
            var val_stx = list[2].AsSyntax();

            var var_id = var_stx.AsIdentifier();
            var value = AstBuilder.ExpandInternal(val_stx, env);

            // Read the variable
            int envIdx = 0;
            var binding = env.LookupAstRecursively(var_id, ref envIdx); 
            
            // TODO! Maybe error when it is not defined

            if (binding == null)
            {
                // Global variable
                var localIdx = env.Define(var_id, new GlobalBinding(var_stx));
                return new AstSet(stx, var_stx, value, localIdx, -1, -1);
            }
            else
            {
                if (envIdx == 0)
                {
                    // Local variable
                    return new AstSet(stx, var_stx, value, binding.VarIdx, 0, 0);
                }
                else
                {
                    // up-value reference
                    if (binding is LocalBinding || binding is ArgumentBinding)
                    {
                        // up value to local variable
                        var localIdx = env.Define(var_id, new UpBinding(var_stx, envIdx, binding.VarIdx));
                        return new AstSet(stx, var_stx, value, localIdx, envIdx, binding.VarIdx);
                    }
                    else if (binding is GlobalBinding)
                    {
                        // global variable
                        var localIdx = env.Define(var_id, new GlobalBinding(var_stx));
                        return new AstSet(stx, var_stx, value, localIdx, 0, 0);
                    }
                    else if (binding is UpBinding)
                    {
                        // upValue to other upValue
                        var upBinding = binding as UpBinding;
                        var nEnvIdx = upBinding.UpEnvIdx + envIdx;
                        var nVarIdx = upBinding.UpVarIdx;
                        var localIdx = env.Define(var_id, new UpBinding(var_stx, nEnvIdx, nVarIdx));
                        return new AstSet(stx, var_stx, value, localIdx, nEnvIdx, nVarIdx);
                    }
                    else
                    {
                        throw new SystemException();
                    }
                }
            }
        }
    }
}