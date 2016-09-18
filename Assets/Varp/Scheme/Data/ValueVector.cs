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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VARP.Scheme.Data1
{
    using Data;
    public struct ValueVector
    {
        public List<Value> vector;

        public ValueVector(IEnumerable<Value> collection) 
        {
            vector = new List<Value>(collection);
        }
        public ValueVector(int capacity) 
        {
            vector = new List<Value>(capacity);
        }
        public ValueVector(IEnumerable<object> collection) 
        {
            vector = new List<Value>();
            foreach (var o in collection)
                vector.Add(new Value(o));
        }
        public Value ToValue()
        {
            return new Value(this);
        }

        public static ValueVector VectorFromArguments(params Value[] args)
        {
            return new ValueVector(args);
        }
        public static ValueVector VectorFromArguments(params object[] args)
        {
            return new ValueVector(args);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool appendSpace = false;
            sb.Append("#(");
            foreach (var v in this)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(ValueString.ToString(v));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }

        #region List<T>

        public Value this[int index] {
            get { return vector[index]; }
            set { vector[index] = value; }
        }

        public int Capacity { get { return vector.Capacity; } set { vector.Capacity = value; } }
        public int Count { get { return vector.Capacity; } }

        public void Add(Value item) { vector.Add(item); }
        public void AddRange(IEnumerable<Value> collection) { vector.AddRange(collection); }
        public ReadOnlyCollection<Value> AsReadOnly() { return vector.AsReadOnly(); }
        public int BinarySearch(Value item) { return vector.BinarySearch(item); }
        public int BinarySearch(Value item, IComparer<Value> comparer) { return vector.BinarySearch(item, comparer); }
        public int BinarySearch(int index, int count, Value item, IComparer<Value> comparer) { return vector.BinarySearch(index, count, item, comparer); }
        public void Clear() { vector.Clear(); }
        public bool Contains(Value item) { return vector.Contains(item); }
        public List<TOutput> ConvertAll<TOutput>(Converter<Value, TOutput> converter) { return vector.ConvertAll<TOutput>(converter); }
        public void CopyTo(Value[] array) { vector.CopyTo(array); }
        public void CopyTo(Value[] array, int arrayIndex) { vector.CopyTo(array, arrayIndex); }
        public void CopyTo(int index, Value[] array, int arrayIndex, int count) { vector.CopyTo(index,array, arrayIndex, count); }
        public bool Exists(Predicate<Value> match) { return vector.Exists(match); }
        public Value Find(Predicate<Value> match) { return vector.Find(match); }
        public List<Value> FindAll(Predicate<Value> match) { return vector.FindAll(match); }
        public int FindIndex(Predicate<Value> match) { return vector.FindIndex(match); }
        public int FindIndex(int startIndex, Predicate<Value> match) { return vector.FindIndex(startIndex, match); }
        public int FindIndex(int startIndex, int count, Predicate<Value> match) { return vector.FindIndex(startIndex, count, match); }
        public Value FindLast(Predicate<Value> match) { return vector.FindLast(match); }
        public int FindLastIndex(Predicate<Value> match) { return vector.FindLastIndex(match); }
        public int FindLastIndex(int startIndex, Predicate<Value> match) { return vector.FindLastIndex(startIndex, match); }
        public int FindLastIndex(int startIndex, int count, Predicate<Value> match) { return vector.FindLastIndex(startIndex, count, match); }
        public void ForEach(Action<Value> action) { vector.ForEach(action); }
        public List<Value>.Enumerator GetEnumerator() { return vector.GetEnumerator(); }
        public List<Value> GetRange(int index, int count) { return vector.GetRange(index, count); }
        public int IndexOf(Value item) { return vector.IndexOf(item); }
        public int IndexOf(Value item, int index) { return vector.IndexOf(item, index); }
        public int IndexOf(Value item, int index, int count) { return vector.IndexOf(item, index, count); }
        public void Insert(int index, Value item) { vector.Insert(index, item); }
        public void InsertRange(int index, IEnumerable<Value> collection) { vector.InsertRange(index, collection); }
        public int LastIndexOf(Value item) { return vector.LastIndexOf(item); }
        public int LastIndexOf(Value item, int index) { return vector.LastIndexOf(item, index); }
        public int LastIndexOf(Value item, int index, int count) { return vector.LastIndexOf(item, index, count); }
        public bool Remove(Value item) { return vector.Remove(item); }
        public int RemoveAll(Predicate<Value> match) { return vector.RemoveAll(match); }
        public void RemoveAt(int index) { vector.RemoveAt(index); }
        public void RemoveRange(int index, int count) { vector.RemoveRange(index, count); }
        public void Reverse() { vector.Reverse(); }
        public void Reverse(int index, int count) { vector.Reverse(index, count); }
        public void Sort() { vector.Sort(); }
        public void Sort(IComparer<Value> comparer) { vector.Sort(comparer); }
        public void Sort(Comparison<Value> comparison) { vector.Sort(comparison); }
        public void Sort(int index, int count, IComparer<Value> comparer) { vector.Sort(index, count, comparer); }
        public Value[] ToArray() { return vector.ToArray(); }
        public void TrimExcess() { vector.TrimExcess(); }
        public bool TrueForAll(Predicate<Value> match) { return vector.TrueForAll(match); }

        #endregion
    }
}
