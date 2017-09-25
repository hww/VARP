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
    using VARP.Scheme.VM;

    public sealed class PrimitiveIf : BasePrimitive
    {
        // (if () ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            var list = stx.AsLinkedList<Value>();
            var argc = GetArgsCount(list);
            AssertArgsMinimum("if", "arity mismatch", 2, argc, list, stx);

            var keyword = list[0].AsSyntax();
            var cond = list[1].AsSyntax();
            var then = list[2].AsSyntax();
            var elsc = argc < 3 ? null : list[3].AsSyntax();

            var cond_ast = AstBuilder.ExpandInternal(cond, env);
            var then_ast = AstBuilder.ExpandInternal(then, env);
            var else_ast = AstBuilder.ExpandInternal(elsc, env);

            return new AstConditionIf(stx, keyword, cond_ast, then_ast, else_ast);
        }
    }
}