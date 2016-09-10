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
    using Data;

    public sealed class PrimitiveLet : BasePrimitive
    {
        // (let () ...)
        public static AST Expand(Syntax stx, LexicalEnvironment env)
        {
            Pair list = stx.GetList();
            int argc = GetArgsCount(list);
            AssertArgsMinimum(stx, 2, argc, "lambda:");

            Syntax keyword = list[0] as Syntax;     // let arguments
            Syntax arguments = list[1] as Syntax;   // let arguments
            Pair body = list.PairAtIndex(2);        // let body

            if (!arguments.IsSyntaxExpression) throw new SyntaxError(stx, "let: bad syntax (missing name or binding pairs)");

            Arguments letarguments = new Arguments();
            ArgumentsList.ParseLetList(arguments.GetList(), env, ref letarguments);

            AST lambda = new AstLambda(stx, keyword, letarguments, AstBuilder.ExpandListElements(body, env));
            return new AstApplication(stx, new Pair(lambda, letarguments.values));
        }
    }
}