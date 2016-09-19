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
    using Exception;
    using Data;

    public sealed class PrimitiveLet : BasePrimitive
    {
        // (let () ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            LinkedList<Value> list = stx.AsLinkedList<Value>();
            int argc = GetArgsCount(list);
            AssertArgsMinimum("let", "arity mismatch", 2, argc, list, stx);

            Syntax keyword = list[0].AsSyntax();     // let arguments
            Syntax arguments = list[1].AsSyntax();   // let arguments

            if (!arguments.IsExpression) throw SchemeError.SyntaxError("let", "bad syntax (missing name or binding pairs)", stx);

            LetArguments letarguments = new LetArguments();
            ArgumentsList.ParseLetList(stx, arguments.AsLinkedList<Value>(), env, ref letarguments);
            Environment localEnv = env.CreateEnvironment(stx, letarguments);

            AST lambda = new AstLambda(stx, keyword, letarguments, AstBuilder.ExpandListElements(list, 2, localEnv));
            LinkedList<Value> result = new LinkedList<Value>();
            result.AddLast(lambda.ToValue());
            result.Append(letarguments.values);
            return new AstApplication(stx, result);
        }
    }
}