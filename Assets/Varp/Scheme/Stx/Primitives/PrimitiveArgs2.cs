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

    public sealed class PrimitiveArgs2 : BasePrimitive
    {
        // two arguments primitive.
        // more that two arguments will be raped to list of primitives
        // (foo 1 2)
        public static AST Expand(Syntax stx, Environment env)
        {
            LinkedList<Value> list = stx.AsLinkedList<Value>();
            int argc = GetArgsCount(list);
            AssertArgsMinimum("primitive2", "arity mismatch", 2, argc, list, stx);
            Syntax set_kwd = list[0].AsSyntax();
            LinkedList<Value> arguments = AstBuilder.ExpandListElements(list, 1, env);
            if (argc == 2)
            {
                return new AstPrimitive(stx, set_kwd, arguments);
            }
            else
            {
                // for expression (+ 1 2 3 4)
                LinkedList<Value> args = arguments.DuplicateReverse(0,-1) as LinkedList<Value>;     //< (+ 4 3 2 1)
                AST rightarg = args[0].AsAST();                                     //< 4
                int skip = 1;
                foreach (Value leftarg in args)                                     //< 3, 2, 1, 
                {
                    if (skip-- > 0) continue;
                    rightarg = new AstPrimitive(stx, set_kwd, ValueLinkedList.FromArguments(leftarg, rightarg));
                }
                return rightarg;
            }
        }
    }
}