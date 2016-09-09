﻿/* 
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

    using Data;

    public class PrimitiveIf : BasePrimitive
    {
        // (if () ...)
        public static AST Expand(Syntax stx, LexicalEnvironment env)
        {
            Pair list = stx.GetList();
            int argc = GetArgsCount(list);
            AssertArgsMinimum(stx, 2, argc, "if:");

            Syntax keyword = list[0] as Syntax;
            Syntax cond = list[1] as Syntax;
            Syntax then = list[2] as Syntax;
            Syntax elsc = argc < 3 ? null : list[3] as Syntax;

            AST cond_ast = AstBuilder.Expand(cond, env);
            AST then_ast = AstBuilder.Expand(then, env);
            AST else_ast = AstBuilder.Expand(elsc, env);

            return new AstIfCondition(stx, keyword, cond_ast, then_ast, else_ast);
        }
    }
}