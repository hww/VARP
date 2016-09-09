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
using System.Text;
using System;
using UnityEngine;

namespace VARP.DataStructures
{
    public class DoubleLink<T> : IEnumerable<DoubleLink<T>>, IComparable<DoubleLink<T>> where T : IComparable<T>
    {
        // Check if this element is root of list. Root element
        // Used as the pointer to the list. There can be one one
        // Root element in the list. Root
        public virtual bool IsEnd { get; set; }

        public T Data;
        public DoubleLink<T> Next;          //< Link to next element
        public DoubleLink<T> Previous;      //< Link to previous element

        // ====================================================================================
        // Constructors.
        // ====================================================================================
        public DoubleLink() : this(default(T)) { }
        // Create empty link without connection to the list
        public DoubleLink(T data) : this(data, null, null) { }
        // Create link without connection to the list
        public DoubleLink(T data, DoubleLink<T> next, DoubleLink<T> previous)
        {
            Data = data;
            Next = next;
            Previous = previous;
        }

        // ====================================================================================
        // Methods
        // ====================================================================================

        // Remove link from list
        public void Unlink()
        {
            if (Next != null)
                Next.Previous = Previous;
            if (Previous != null)
                Previous.Next = Next;
            Next = Previous = null;
        }
        // Adds this element to a list, before the given element.
        public void LinkBefore(DoubleLink<T> before)
        {
            Debug.Assert(before != null);

            Previous = before.Previous;
            before.Previous = this;
            Next = before;
            if (Previous != null)
                Previous.Next = this;
        }
        // Adds this element to the list, after the specified element
        public void LinkAfter(DoubleLink<T> after)
        {
            Debug.Assert(after != null);

            Next = after.Next;
            after.Next = this;
            Previous = after;
            if (Next != null)
                Next.Previous = this;
        }
        // Adds this element to the list, with replacing the specified element.
        // This is equivalent to calling the sequence: LinkBefore(Replace); replace.Unlink();
        public void LinkReplace(DoubleLink<T> replace)
        {
            Debug.Assert(replace != null);

            DoubleLink<T> replacePrev = replace.Previous;
            DoubleLink<T> replaceNext = replace.Next;

            Previous = replacePrev;
            Next = replaceNext;

            if (Previous != null)
                Previous.Next = this;

            if (Next != null)
                Next.Previous = this;

            replacePrev = null;
            replaceNext = null;
        }
        // Check if link is linked
        public bool IsLinked
        {
            get { return Next != null || Previous != null; }
        }

        // ====================================================================================
        // Interfaces
        // ====================================================================================

        #region IComparable
        public int CompareTo(DoubleLink<T> other)
        {
            if (other == null) return -1;
            return this.Data.CompareTo(other.Data);
        }
        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<DoubleLink<T>> GetEnumerator()
        {
            DoubleLink<T> node = this;
            while (node != null && !node.IsEnd)
            {
                yield return node;
                node = node.Next;
            }
        }

        public IEnumerator<DoubleLink<T>> GetEnumeratorReversed()
        {
            DoubleLink<T> node = this;
            while (node != null && !node.IsEnd)
            {
                yield return node;
                node = node.Previous;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }

        #endregion
    }
    /*
    public class IntrusiveLink<T> : DoubleLinkBase<T> where T:class 
    {
        public override bool IsEnd { get { return false; } }

        public T Data { get { return this as T; } set {  } }

        // Create empty link without connection to the list
        public IntrusiveLink() : this(null, null) { }
        // Create link without connection to the list
        public IntrusiveLink(IntrusiveLink<T> next, IntrusiveLink<T> previous) : base(next, previous)
        {
        }
    }

    public class DoubleLink<T> : DoubleLinkBase<T> 
    {
        protected T data;
        public override bool IsEnd { get { return false; } }
        public  T Data { get { return data; } set { data = value; } }
        public DoubleLink() : this(default(T)) { }
        public DoubleLink(T data) : this(data, null, null) { }
        // Create link without connection to the list
        public DoubleLink(T data, DoubleLink<T> next, DoubleLink<T> previous) : base(next, previous)
        {
            this.data = data;
        }

    }
    */
    // Used for list class 
    public class DoubleLinkRoot<T> : DoubleLink<T> where T : IComparable<T>
    {
        public override bool IsEnd { get { return true; } }

    }

    public class DoubleLinkedList<T> : IEnumerable<DoubleLink<T>> where T : IComparable<T>
    {
        DoubleLinkRoot<T> Root = new DoubleLinkRoot<T>();
        public DoubleLink<T> First { get { return Root.Next; } set { Root.Next = value; } }
        public DoubleLink<T> Last { get { return Root.Previous; } set { Root.Previous = value; } }

        public DoubleLinkedList()
        {
            First = Last = Root;
        }
        // Unlink @link from the list
        public void Unlink(DoubleLink<T> link)
        {
            Debug.Assert(link != null);
            link.Unlink();
        }
        // Adds this element to a list, before the given element.
        public void LinkBefore(DoubleLink<T> link, DoubleLink<T> before)
        {
            Debug.Assert(link != null);
            Debug.Assert(before != null);
            link.LinkBefore(before);
        }
        // Adds this element to the list, after the specified element
        public void LinkAfter(DoubleLink<T> link, DoubleLink<T> after)
        {
            Debug.Assert(link != null);
            Debug.Assert(after != null);
            link.LinkAfter(after);
        }
        // Adds @link element to the list, with replacing the specified element.
        // This is equivalent to calling the sequence: LinkBefore(Replace); replace.Unlink();
        public void LinkReplace(DoubleLink<T> link, DoubleLink<T> replace)
        {
            Debug.Assert(link != null);
            Debug.Assert(replace != null);
            if (link == replace) return;
            link.LinkReplace(replace);
        }
        // Add first element
        public void AddLast(DoubleLink<T> link)
        {
            Debug.Assert(link != null);
            if (link.IsLinked) link.Unlink();
            link.LinkAfter(Last);
        }
        // Add last element
        public void AddFirst(DoubleLink<T> link)
        {
            Debug.Assert(link != null);
            if (link.IsLinked) link.Unlink();
            link.LinkBefore(First);
        }
        // Check if list is empty
        public bool IsEmpty
        {
            get { return First == Root; }
        }
        // Clear list
        public void Clear()
        {
            while (!IsEmpty) First.Unlink();
        }
        // Get element with @index 
        private DoubleLink<T> GetAt(int index)
        {
            if (IsEmpty) return null;
            var node = First;
            for (int i = 0; i < index; i++)
            {
                if (node.IsEnd)
                    return null;
                node = node.Next;
            }
            return node;
        }

        // Get element with index as the [] operator
        public DoubleLink<T> this[int index]
        {
            get
            {
                var node = GetAt(index);
                if (node == null)
                    throw new System.ArgumentOutOfRangeException();
                return node;
            }
            set
            {
                var node = GetAt(index);
                if (node == null)
                    throw new System.ArgumentOutOfRangeException();
                LinkReplace(value, node);
            }
        }
        // Convert list to the List<T>
        public List<T> ToList()
        {
            List<T> list = new List<T>();
            var node = First;
            while (!node.IsEnd)
            {
                list.Add(node.Data);
                node = node.Next;
            }
            return list;
        }
        // Convert list to array
        public T[] ToArray()
        {
            return ToList().ToArray();
        }
        // Join elements to the string
        public string Join(string delimeter)
        {
            StringBuilder sb = new StringBuilder();
            var node = First;
            if (!node.IsEnd)
            {
                sb.Append(node.Data.ToString());
                node = node.Next;
            }
            while (!node.IsEnd)
            {
                sb.Append(delimeter);
                sb.Append(node.Data.ToString());
                node = node.Next;
            }
            return sb.ToString();
        }
        // Convert this list to string
        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var node = First;
            sb.Append("(");
            while (!node.IsEnd)
            {
                sb.Append(node.Data.ToString());
                node = node.Next;
            }
            sb.Append(")");
            return sb.ToString();
        }
        /// <summary>
        /// Returns the list items as a readable multi--line string.
        /// </summary>
        /// <returns></returns>
        public virtual string ToReadable()
        {
            string listAsString = string.Empty;
            int i = 0;
            var node = First;

            while (!node.IsEnd)
            {
                listAsString = string.Format("{0}[{1}] => {2}\r\n", listAsString, i, node.Data.ToString());
                node = node.Next;
                ++i;
            }

            return listAsString;
        }

        #region IEnumerable<T> Members

        public IEnumerator<DoubleLink<T>> GetEnumerator()
        {
            DoubleLink<T> node = First;
            while (node != null && !node.IsEnd)
            {
                yield return node as DoubleLink<T>;
                node = node.Next;
            }
        }

        public IEnumerator<DoubleLink<T>> GetEnumeratorReversed()
        {
            DoubleLink<T> node = Last;
            while (node != null && !node.IsEnd)
            {
                yield return node as DoubleLink<T>;
                node = node.Previous;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }

        #endregion

    }

}