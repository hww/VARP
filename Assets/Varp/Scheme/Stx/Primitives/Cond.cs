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
    using Exception;

    public sealed class PrimitiveCond : BasePrimitive
    {
        // (cond () ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            ValueList list = stx.AsValueList();    //< list of syntax objects
            int argc = GetArgsCount(list);

            Syntax keyword = list[0].AsSyntax();
            ValueList allcases = null;
            ValueList elsecase = null;

            LinkedListNode<Value> curent = list.GetNodeAtIndex(1);

            while (curent!=null)
            {
                Syntax conditional_stx = curent.Value.AsSyntax();

                if (elsecase != null)
                    throw SchemeError.SyntaxError("cond", "unexpected expression after condition's else clause", conditional_stx);

                if (conditional_stx.IsExpression)
                {
                    // Get single conditional expression
                    ValueList conditional_list = conditional_stx.AsValueList();
                    
                    // Check arguments count, should be 2 for each condition
                    int size = conditional_list.Count;
                    if (size != 2) throw SchemeError.ArityError("cond", "arity mismatch", 2, size, conditional_list, conditional_stx);
                    
                    // Now get condition and it's expression
                    Syntax var = conditional_list[0].AsSyntax();
                    Syntax val = conditional_list[1].AsSyntax();

                    if (var.IsIdentifier && var.AsIdentifier() == Symbol.ELSE)
                    {
                        elsecase = ValueList.ListFromArguments(var, AstBuilder.Expand(val, env));
                    }
                    else
                    {
                        AST cond_ = AstBuilder.Expand(var, env);
                        AST then_ = AstBuilder.Expand(val, env);
                        ValueList single_cond = ValueList.ListFromArguments(cond_, then_);
                        allcases.AddLast(new Value(single_cond));
                    }
                }
                else
                {
                    throw SchemeError.SyntaxError("cond", "Expected condition's expression list", conditional_stx);
                }
                curent = curent.Next;
            }

            return new AstConditionCond(stx, keyword, allcases, elsecase);
        }
    }
}