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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace VARP.Scheme.Data
{
    using DataStructures;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class ValuePair : SObject
    {
        public Value Item1;
        public Value Item2;
        public ValuePair(Value item1, Value item2) 
        {
            Item1.Set(item1);
            Item2.Set(item2);
        }

        public ValuePair(object item1, object item2)
        {
            Item1.Set(item1);
            Item2.Set(item2);
        }

        public ValuePair(int item1, int item2)
        {
            Item1.Set(item1);
            Item2.Set(item2);
        }
        public ValuePair(double item1, double item2)
        {
            Item1.Set(item1);
            Item2.Set(item2);
        }

        #region Dictionaries or check equality

        private static readonly IEqualityComparer<Value> Item1Comparer = EqualityComparer<Value>.Default;
        private static readonly IEqualityComparer<Value> Item2Comparer = EqualityComparer<Value>.Default;

        public override int GetHashCode()
        {
            var hc = 0;
            if (!ReferenceEquals(Item1, null))
                hc = Item1Comparer.GetHashCode(Item1);
            if (!ReferenceEquals(Item2, null))
                hc = (hc << 3) ^ Item2Comparer.GetHashCode(Item2);
            return hc;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Tuple<Value, Value>;
            if (ReferenceEquals(other, null))
                return false;
            else
                return Item1Comparer.Equals(Item1, other.Item1) && Item2Comparer.Equals(Item2, other.Item2);
        }
        #endregion

        #region String-based formatting
        public override string ToString()
        {
            return string.Format("({0} . {1})", Item1.ToString(), Item2.ToString());
            // { return ToString(null, CultureInfo.CurrentCulture); }
        }
        public string ToString(string format, System.IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format ?? "{0},{1}", Item1, Item2);
        }
        #endregion

        #region DebuggerDisplay 
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("#<ValuePair ({0} . {1})>", ValueString.ToString(Item1.DebuggerDisplay), ValueString.ToString(Item1.DebuggerDisplay));
            }
        }
        #endregion
    }

}
