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

    public sealed class PrimitiveArgs2 : BasePrimitive
    {
        // two arguments primitive.
        // more that two arguments will be raped to list of primitives
        // (foo 1 2)
        public static AST Expand(Syntax stx, Environment env)
        {
            var list = stx.AsLinkedList<Value>();
            var argc = GetArgsCount(list);
            AssertArgsMinimum("primitive2", "arity mismatch", 2, argc, list, stx);
            var set_kwd = list[0].AsSyntax();
            var arguments = AstBuilder.ExpandListElements(list, 1, env);
            if (argc == 2)
            {
                return new AstPrimitive(stx, set_kwd, arguments);
            }
            else
            {
                // for expression (+ 1 2 3 4)
                var args = arguments.DuplicateReverse(0,-1);     //< (+ 4 3 2 1)
                var rightarg = args[0];                                     //< 4
                var skip = 1;
                foreach (var leftarg in args)                                     //< 3, 2, 1, 
                {
                    if (skip-- > 0) continue;
                    var values = ValueLinkedList.FromArguments(leftarg, rightarg);
                    var prim = new AstPrimitive(stx, set_kwd, values);
                    rightarg.Set(prim);
                }
                return rightarg.AsAST();
            }
        }
    }
}