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
using VARP.Interfaces;
using System.Collections.Generic;

namespace VARP.DataStructures
{
    internal class SingleNodeBase<TNodeType> : IEnumerable<SingleNodeBase<TNodeType>> where TNodeType : class
    {
        public SingleNodeBase<TNodeType> next;

        public SingleNodeBase(SingleNodeBase<TNodeType> next)
        {
            this.next = next;
        }
        public TNodeType Super {  get { return this as TNodeType; } }

        #region IEnumerable<T> Members

        public IEnumerator<SingleNodeBase<TNodeType>> GetEnumerator()
        {
            var node = this;
            while (node != null)
            {
                yield return node;
                node = node.next;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return GetEnumerator();
        }

        #endregion
    }


    // Simple single-linked list template for Structure.
    internal class SListNode<T> : SingleNodeBase<SListNode<T>>, IListNode<T> where T : struct
    {
        public T element;

        // Constructor.
        private SListNode(T inElement, SListNode<T> inNext) : base(inNext)
        {
            element = inElement;
        }

        public T Data
        {
            get { return element; }
        }
    }

    // Simple single-linked list template for class
    internal class CIntrusiveListNode<T> : SingleNodeBase<CIntrusiveListNode<T>>, IListNode<T> where T : class
    {
        // Constructor.
        public CIntrusiveListNode(CIntrusiveListNode<T> inNext = null) : base(inNext)
        {
        }

        public T Data
        {
            get { return this as T; }
        }
    }

    // Simple single-linked list template for class
    internal class CListNode<T> : SingleNodeBase<CListNode<T>>, IListNode<T>  where T : class
    {
        public T element;

        // Constructor.

        public CListNode(T inElement, CListNode<T> inNext = null) : base(inNext)
        {
            element = inElement;
        }

        public T Data
        {
            get { return element; }
        }

    }
}