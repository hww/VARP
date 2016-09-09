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

namespace VARP.Scheme.Data
{
    public class SVectorBase<T> : SObject, IEnumerable<T>, ICollection<T>, IList<T> where T:class
    {
        public List<T> Value;
        public SVectorBase() { Value = new List<T>(); }
        public SVectorBase(int capacity) { Value = new List<T>(capacity); }
        public SVectorBase(List<T> val) { Value = val; }
        public int Count { get { return Value == null ? 0 : Value.Count; } }

        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj)
        {
            if (obj == null && Value == null) return true;
            if (obj is SVectorBase<T>) return Equals(obj as SVectorBase<T>);
            return false;
        }
        public bool Equals(SVectorBase<T> obj)
        {
            if (obj == null && Value == null) return true;
            if (obj == null) return false;
            if (Value == null) return false;
            if (Value.Count != obj.Count) return false;
            for (int i = 0; i < Value.Count; i++)
                if (Value[i] != obj[i]) return false;
            return true;
        }
        public override string ToString() { return string.Format("#<array size={0}>", Count); }

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string AsString() { return ToString(); }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var b in Value)
                yield return b;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection<T>

        public T this[int index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public void Add(T value) { Value.Add(value); }
        public bool Contains(T value) { return Value.Contains(value); }
        public bool Remove(T value) { return Value.Remove(value); }
        public void Clear() { Value.Clear(); }
        public void CopyTo(T[] dst, int idx) { Value.CopyTo(dst, idx); }
        public bool IsReadOnly { get { return false; } }
        #endregion

        #region IList<T>
        public int IndexOf(T value) { return Value.IndexOf(value); }
        public void Insert(int idx, T value) { Value.Insert(idx, value); }
        public void RemoveAt(int idx) { Value.RemoveAt(idx); }
        #endregion
    }

    public class SVector : SVectorBase<SObject>
    {
        public SVector() : base() {  }
        public SVector(int capacity) : base(capacity) { }
        public SVector(List<SObject> val) : base(val) { }


    }
}
