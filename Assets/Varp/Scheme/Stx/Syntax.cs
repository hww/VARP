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

using System.Collections;
using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using Data;
    using Exception;
    using Tokenizing;

    public sealed class Syntax : SObject, IEnumerable<Syntax>
    {
        public SObject expression;
        public Location location;

        public Syntax() : base()
        {
        }
        public Syntax(SObject expression, Location location) 
        {
            this.expression = expression;
            this.location = location;
        }
        public Syntax(SObject expression, Token token)
        {
            this.expression = expression;
            this.location = token == null ? null : token.location;
        }


        #region List Methods
        public Pair GetList() { return SObjectCast.GetPairOrNull(expression); }
        public int Count { get { return Pair.Length(expression); } }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<Syntax> GetEnumerator()
        {
            if (expression is Pair)
            {
                Pair c = expression as Pair;
                while (c != null)
                {
                    yield return c.Car as Syntax;
                    c = c.Cdr as Pair;
                }
            }
        }
        #endregion

        public SObject GetDatum() { return SyntaxToDatum(expression); }
        public T GetDatum<T>() where T:class { return SyntaxToDatum(expression) as T; }
        public Symbol GetIdentifier()
        {
            if (expression is Symbol) return expression as Symbol;
            throw new ExpectedIdentifier(this);
        }
        static SObject SyntaxToDatum(SObject expression)
        {
            if (expression is Syntax)
                return (expression as Syntax).GetDatum();
            if (expression is Pair)
            {
                Pair curent = expression as Pair;
                Pair first = new Pair(); 
                Pair last = first;
                while (curent != null)
                {
                    last.Car = SyntaxToDatum(curent.Car);
                    if (curent.Cdr is Pair)
                    {
                        last.Cdr = new Pair();
                        last = last.Cdr as Pair;
                        curent = curent.Cdr as Pair;
                    }
                    else
                    {
                        last.Cdr = SyntaxToDatum(curent.Cdr);
                        curent = null;
                    }
                }
                return first;

            }
            if (expression is SVector)
            {
                SVector src = expression as SVector;
                SVector dst= new SVector(src.Count);
                foreach (Syntax v in src)
                {
                    dst.Add(v.GetDatum());
                }
                return dst;
            }
            return expression;
        }

        public bool IsSyntax(System.Type type) { return this.GetType() == type; }
        public bool IsSyntaxIdentifier { get { return (expression != null) && expression.IsIdentifier; } }
        public bool IsSyntaxLiteral { get { return (expression != null) && expression.IsLiteral; } }
        public bool IsSyntaxExpression { get { return (expression == null) || expression is Pair; } }

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string AsString() { return expression == null ? "()" : expression.AsString(); }

        #endregion


    }
}