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

    using Data;
    using Exception;

    public sealed class PrimitiveCond : BasePrimitive
    {
        // (cond () ...)
        public static AST Expand(Syntax stx, Environment env)
        {
            Pair list = stx.GetList();    //< list of syntax objects
            int argc = GetArgsCount(list);

            Syntax keyword = list.Car as Syntax;
            Pair elsecase = null;
            Pair first = null;
            Pair last = null;

            foreach (Syntax conditional_stx in list.Cdr as Pair)
            {
                if (elsecase != null) throw SchemeError.SyntaxError("cond", "unexpected expression after condition's else clause", conditional_stx);
                if (conditional_stx.IsSyntaxExpression)
                {
                    // Get single conditional expression
                    Pair conditional_list = conditional_stx.GetList();
                    // Check arguments count, should be 2 for each condition
                    int size = Pair.Length(conditional_list);
                    if (size != 2) throw SchemeError.ArityError("cond", "arity missmach", 2, size, conditional_list, conditional_stx);
                    // Now get condition and it's expression
                    Syntax var = conditional_list[0] as Syntax;
                    Syntax val = conditional_list[1] as Syntax;

                    if (var.IsSyntaxIdentifier && var.GetIdentifier() == Symbol.ELSE)
                    {
                        elsecase = Pair.ListFromArguments(var, AstBuilder.Expand(val, env));
                    }
                    else
                    {
                        AST cond_ = AstBuilder.Expand(var, env);
                        AST then_ = AstBuilder.Expand(val, env);
                        Pair single_cond = Pair.ListFromArguments(cond_, then_);
                        if (last == null)
                            first = last = new Pair();
                        else
                        {
                            last.Cdr = new Pair();
                            last = last.Cdr as Pair;
                        }
                        last.Car = single_cond;
                    }
                }
                else
                {
                    throw SchemeError.SyntaxError("cond", "Expected condition's expression list", conditional_stx);
                }
            }

            return new AstConditionCond(stx, keyword, first, elsecase);
        }
    }
}