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

using UnityEngine;
using System.Collections;
using VARP.Interfaces;
using System.Collections.Generic;
using System.Text;
using VARP.Interfaces;

namespace VARP.DataStructures
{
    class SingleNodeBase<NodeType> : IEnumerable<SingleNodeBase<NodeType>> where NodeType : class
    {
        public SingleNodeBase<NodeType> Next;

        public SingleNodeBase(SingleNodeBase<NodeType> next)
        {
            Next = next;
        }
        public NodeType Super {  get { return this as NodeType; } }

        #region IEnumerable<T> Members

        public IEnumerator<SingleNodeBase<NodeType>> GetEnumerator()
        {
            SingleNodeBase<NodeType> node = this;
            while (node != null)
            {
                yield return node;
                node = node.Next;
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


    // Simple single-linked list template for Structure.
    class SListNode<T> : SingleNodeBase<SListNode<T>>, IListNode<T> where T : struct
    {
        public T Element;

        // Constructor.
        SListNode(T InElement, SListNode<T> InNext) : base(InNext)
        {
            Element = InElement;
        }

        public T Data
        {
            get { return Element; }
        }
    }

    // Simple single-linked list template for class
    class CIntrusiveListNode<T> : SingleNodeBase<CIntrusiveListNode<T>>, IListNode<T> where T : class
    {
        // Constructor.
        public CIntrusiveListNode(CIntrusiveListNode<T> InNext = null) : base(InNext)
        {
        }

        public T Data
        {
            get { return this as T; }
        }
    }

    // Simple single-linked list template for class
    class CListNode<T> : SingleNodeBase<CListNode<T>>, IListNode<T>  where T : class
    {
        public T Element;

        // Constructor.

        public CListNode(T InElement, CListNode<T> InNext = null) : base(InNext)
        {
            Element = InElement;
        }

        public T Data
        {
            get { return Element; }
        }

    }
}