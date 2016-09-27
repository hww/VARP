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

using System;
using System.Collections;
using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using Data;
    using Exception;
    using Tokenizing;
    using DataStructures;
    using REPL;

    public sealed class Syntax : ValueClass
    {
        public readonly static Syntax False = new Syntax(Value.False, null as Location);
        public readonly static Syntax Void = new Syntax(Value.Void, null as Location);

        private Value expression;
        private Location location;

        public Syntax() : base()
        {
        }
        public Syntax(ValueClass expression) : base()
        {
            this.expression = new Value(expression);
        }
        public Syntax(Value expression, Location location)
        {
            this.expression = expression;
            this.location = location;
        }
        public Syntax(Value expression, Token token)
        {
            this.expression = expression;
            this.location = token == null ? null : token.location;
        }
        public Syntax(object expression, Location location)
        {
            this.expression.Set(expression);
            this.location = location;
        }
        public Syntax(object expression, Token token)
        {
            this.expression.Set(expression);
            this.location = token == null ? null : token.location;
        }

        #region Cast Syntax To ... Methods
        /// <summary>
        /// Get expression
        /// </summary>
        /// <returns></returns>
        public LinkedList<Value> AsValueLinkedList() { return expression.AsLinkedList<Value>(); }

        /// <summary>
        /// Get expression
        /// </summary>
        /// <returns></returns>
        public LinkedList<T> AsLinkedList<T>() { return expression.AsLinkedList<T>(); }

        /// <summary>
        /// Get identifier (exception if syntax is not identifier)
        /// </summary>
        /// <returns></returns>
        public Symbol AsIdentifier()
        {
            if (expression.IsSymbol) return expression.AsSymbol();
            throw SchemeError.ArgumentError("get-identifier", "identifier?", this);
        }

        /// <summary>
        /// Get datum
        /// </summary>
        /// <returns></returns>
        public Value GetDatum() { return GetDatum(expression); }

        /// <summary>
        /// Get location of this syntax
        /// </summary>
        public Location Location { get { return location; } }

        #endregion

        #region Datum Extractor Methods
        /// <summary>
        /// Method safely cast the syntax's expression to the Datum
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        static Value GetDatum(Value expression)
        {
            if (expression.IsSyntax)
                expression = (expression.AsSyntax()).expression;

            if (expression.IsLinkedList<Value>())
            {
                LinkedList<Value> result = new LinkedList<Value>();
                foreach (var val in expression.AsLinkedList<Value>())
                    result.AddLast(GetDatum(val));
                return new Value(result);
            }

            if (expression.IsLinkedList<Syntax>())
            {
                LinkedList<Syntax> src = expression.AsLinkedList<Syntax>();
                LinkedList<Value> dst = new LinkedList<Value>();
                foreach (var v in src)
                    dst.AddLast(GetDatum(v.ToValue()));
                return new Value(dst);
            }

            if (expression.IsList<Value>())
            {
                List<Value> src = expression.AsList<Value>();
                List<Value> dst = new List<Value>(src.Count);
                foreach (var v in src)
                {
                    if (v.IsSyntax)
                        dst.Add(GetDatum(v.AsSyntax().ToValue()));
                    else
                        throw SchemeError.ArgumentError("syntax->datum", "identifier?", v);
                }
                return new Value(dst);
            }


            if (expression.IsValuePair)
            {
                ValuePair pair = expression.AsValuePair();
                return new Value(new ValuePair(GetDatum(pair.Item1), GetDatum(pair.Item2)));
            }


                return expression;
        }
        #endregion

        public bool IsSymbol { get { return expression.IsSymbol; } }
        public bool IsIdentifier { get { return (expression.IsSymbol) && expression.AsSymbol().IsIdentifier; } }
        public bool IsLiteral { get { return !IsExpression && !IsIdentifier; } }
        public bool IsExpression { get { return (expression == null) || expression.IsLinkedList<Value>(); } }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return expression == null ? "()" : expression.ToString(); }

        #endregion


        #region DebuggerDisplay 
        public override string DebuggerDisplay
        {
            get
            {
                try
                {
                    return string.Format(Inspector.Inspect(this));
                }
                catch (System.Exception ex)
                {
                    return string.Format("#<syntax ispect-error='{0}'>", ex.Message);
                }
            }
        }
        #endregion

    }
}