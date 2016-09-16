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
using UnityEngine;

namespace VARP.DataStructures
{
    /// <summary>
    /// Advanced list. Has additional features as: count elemens, access to the List class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkBaseAdv<T> : IEnumerable<T> where T : class
    {
        // Check if this element is root of list. Root element
        // Used as the pointer to the list. There can be one one
        // Root element in the list. Root
        public virtual bool IsEnd { get; set; }
        public virtual T Data { get; set; }
        public virtual DoubleLinkedListAdv<T> Root { get; set; }


        public DoubleLinkedListAdv<T> List;                //< Pointer to the list 
        public DoubleLinkBaseAdv<T> Next;          //< Link to next element
        public DoubleLinkBaseAdv<T> Previous;      //< Link to previous element
        private int count;

        // Create link without connection to the list
        public DoubleLinkBaseAdv(T data = null)
        {
            Data = data;
            Next = null;
            Previous = null;
        }
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
        public void LinkBefore(DoubleLinkBaseAdv<T> before)
        {
            Debug.Assert(before != null);
            Debug.Assert(List != null);
            List.LinkBefore(this, before);
        }
        // Adds this element to the list, after the specified element
        public void LinkAfter(DoubleLinkBaseAdv<T> after)
        {
            Debug.Assert(after != null);
            Debug.Assert(List != null);
            List.LinkAfter(this, after);
        }
        // Adds this element to the list, with replacing the specified element.
        // This is equivalent to calling the sequence: LinkBefore(Replace); replace.Unlink();
        public void LinkReplace(DoubleLinkBaseAdv<T> replace)
        {
            Debug.Assert(replace != null);
            Debug.Assert(List != null);
            List.LinkReplace(this, replace);
        }
        // Check if link is linked
        public bool IsLinked
        {
            get { return List!=null; }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            DoubleLinkBaseAdv<T> node = this;
            while (node != null && !node.IsEnd)
            {
                yield return node.Data;
                node = node.Next;
            }
        }

        public IEnumerator<T> GetEnumeratorReversed()
        {
            DoubleLinkBaseAdv<T> node = this;
            while (node != null && !node.IsEnd)
            {
                yield return node.Data;
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

    public class IntrusiveLinkAdv<T> : DoubleLinkBaseAdv<T> where T : class
    {
        public override bool IsEnd { get { return false; } }

        public override T Data { get { return this as T; } set { /* do not set it for intrusive list */ } }
    }

    public class ExtrusiveLinkAdv<T> : DoubleLinkBaseAdv<T> where T : class
    {
        protected T data;
        public override bool IsEnd { get { return false; } }
        public override T Data { get { return data; } set { data = value; } }


    }

    // Used for list class 
    public class DoubleLinkRootAdv<T> : DoubleLinkBaseAdv<T> where T : class
    {
        public override bool IsEnd { get { return true; } }
        public override T Data { get { return null; } set { } }
    }

    public class DoubleLinkedListAdv<T> : IEnumerable<T> where T : class
    {
        DoubleLinkRootAdv<T> Root;
        public DoubleLinkBaseAdv<T> First { get { return Root.Next; } set { Root.Next = value; } }
        public DoubleLinkBaseAdv<T> Last { get { return Root.Previous; } set { Root.Previous = value; } }
        public int count;

        // Construct empty list
        public DoubleLinkedListAdv()
        {
            First = Last = Root; count = 0;
        }

        // Get elements count
        public int Count { get { return count; } }

        // Remove link from list
        public void Unlink(DoubleLinkBaseAdv<T> link)
        {
            Debug.Assert(link != null);
            Debug.Assert(link.List != this);

            if (link.IsLinked) link.Unlink();

            link.List = this;
            if (link.Next != null)
                link.Next.Previous = link.Previous;
            if (link.Previous != null)
                link.Previous.Next = link.Next;
            link.Next = link.Previous = null;
            link.List = null;
            count--;
        }
        // Adds this element to a list, before the given element.
        public void LinkBefore(DoubleLinkBaseAdv<T> link, DoubleLinkBaseAdv<T> before)
        {
            Debug.Assert(link != null);
            Debug.Assert(before != null);
            Debug.Assert(before.List == this);

            if (link.IsLinked) link.Unlink();

            link.List = this;
            link.Previous = before.Previous;
            before.Previous = link;
            link.Next = before;
            if (link.Previous != null)
                link.Previous.Next = link;
            count++;
        }
        // Adds this element to the list, after the specified element
        public void LinkAfter(DoubleLinkBaseAdv<T> link, DoubleLinkBaseAdv<T> after)
        {
            Debug.Assert(link != null);
            Debug.Assert(after != null);
            Debug.Assert(after.List == this);

            if (link.IsLinked) link.Unlink();

            link.List = this;
            link.Next = after.Next;
            after.Next = link;
            link.Previous = after;
            if (link.Next != null)
                link.Next.Previous = link;
            count++;
        }
        // Adds this element to the list, with replacing the specified element.
        // This is equivalent to calling the sequence: LinkBefore(Replace); replace.Unlink();
        public void LinkReplace(DoubleLinkBaseAdv<T> link, DoubleLinkBaseAdv<T> replace)
        {
            Debug.Assert(link != null);
            Debug.Assert(replace != null);
            Debug.Assert(replace.List == this);

            if (link.IsLinked) link.Unlink();

            link.Previous = replace.Previous;
            link.Next = replace.Next;

            if (link.Previous != null)
                link.Previous.Next = link;

            if (link.Next != null)
                link.Next.Previous = link;

            replace.Next = null;
            replace.Previous = null;
        }
        public void AddLast(DoubleLinkBaseAdv<T> link)
        {
            LinkBefore(link, First);
        }

        public void AddFirst(DoubleLinkBaseAdv<T> link)
        {
            LinkAfter(link, Last);
        }

        public bool IsEmpty
        {
            get { return First == Root; }
        }

        public void Clear()
        {
            while (!IsEmpty) First.Unlink();
        }

        private DoubleLinkBaseAdv<T> GetAt(int index)
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


        public T this[int index]
        {
            get
            {
                var node = GetAt(index);
                if (node == null)
                    throw new System.ArgumentOutOfRangeException();
                return node.Data;
            }
            set
            {
                var node = GetAt(index);
                if (node == null)
                    throw new System.ArgumentOutOfRangeException();
                node.Data = value;
            }
        }

        // Convert to List<T>
        public List<T> ToList()
        {
            List<T> list = new List<T>(count);
            var node = First;
            while (!node.IsEnd)
            {
                list.Add(node.Data);
                node = node.Next;
            }
            return list;
        }
        // Convert to array
        public T[] ToArray()
        {
            T[] list = new T[count];
            var node = First; int i = 0;
            while (!node.IsEnd)
            {
                list[i++] = node.Data;
                node = node.Next;
            }
            return list;
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

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            DoubleLinkBaseAdv<T> node = First;
            while (node != null && !node.IsEnd)
            {
                yield return node.Data;
                node = node.Next;
            }
        }

        public IEnumerator<T> GetEnumeratorReversed()
        {
            DoubleLinkBaseAdv<T> node = Last;
            while (node != null && !node.IsEnd)
            {
                yield return node.Data;
                node = node.Previous;
            }
        }
        // Convert to single line string
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