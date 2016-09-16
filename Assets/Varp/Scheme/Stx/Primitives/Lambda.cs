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
    using DataStructures;

    public sealed class PrimitiveLambda : BasePrimitive
    {
        // (lambda () ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            ValueList list = stx.AsList();
            int argc = GetArgsCount(list);
            AssertArgsMinimum("lambda", "arity mismatch", 1, argc, list, stx);

            Syntax kwdr = list[0].AsSyntax();
            Syntax args = list[1].AsSyntax();

            Arguments arguments = new Arguments();
            ArgumentsList.Parse(stx, args.AsList(), env, ref arguments);

            Environment localEnv = env.CreateEnvironment(stx, arguments);

            ValueList body = new ValueList();

            if (argc > 1)
            {
                LinkedListNode<Value> curent = list.GetNodeAtIndex(2);
                while (curent != null)
                {
                    body.AddLast(AstBuilder.ExpandExpression(curent.Value.AsSyntax(), localEnv).ToValue());
                    curent = curent.Next;
                }
            }
            return new AstLambda(stx, kwdr, arguments, body);
        }
    }
}