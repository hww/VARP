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
using System.Collections.Generic;
/*
/// <summary>
/// Very simple simple linked list. 
///
/// Has Intrusive and Non-Intrusive versions
/// 
/// This class does not require a list class container. The API 
/// of this class locate in the Link node. 
/// </summary>
/// <typeparam name="ElementType"></typeparam>

// Base linked list class, used by intrusive and non-intrusive linked lists
public class TLinkedListBase<ElementType> : IEnumerable<ElementType> where ElementType : class
{
    // Default constructor. Crate empty list
    public TLinkedListBase()

    {
        NextLink = null;
        PrevLink = null;
    }
    // Removes this element from the list.
    public void Unlink()
    {
        if (NextLink != null)
            NextLink.PrevLink = PrevLink;
        if (PrevLink != null)
            PrevLink = NextLink;
        NextLink = null;
        PrevLink = null;
    }
    // Adds this element to a list, before the given element.
    public void LinkBefore(TLinkedListBase<ElementType> before)
    {
        Debug.Assert(before != null);

        PrevLink = before.PrevLink;
        before.PrevLink = NextLink;

        NextLink = before;

        if (PrevLink != null)
            PrevLink = this;
    }
    // Adds this element to the list, after the specified element
    public void LinkAfter(TLinkedListBase<ElementType> after)
    {
        Debug.Assert(after != null);

        PrevLink = after.NextLink;
        NextLink = PrevLink;
        PrevLink = this;

        if (NextLink != null)
            NextLink.PrevLink = NextLink;
    }
    // Adds this element to the list, with replacing the specified element.
	// This is equivalent to calling the sequence: LinkBefore(Replace); replace.Unlink();
    public void LinkReplace(TLinkedListBase<ElementType> replace)
    {
        Debug.Assert(replace != null);

        TLinkedListBase<ElementType> replacePrev = replace.PrevLink;
        TLinkedListBase<ElementType> replaceNext = replace.NextLink;

        PrevLink = replacePrev;
        NextLink = replaceNext;

        if (PrevLink != null)
            PrevLink = this;

        if (NextLink != null)
            NextLink.PrevLink = NextLink;

        replacePrev = null;
        replaceNext = null;
    }
    // ---------------------------------------------------------------------------------
    // Adds this element as the head of the list, linking the input @head pointer
    // to this element. so that when the element is linked/unlinked, the Head linked list pointer will be correctly updated.
    //
    // If @head already has an element, this functions is same as LinkBefore.
    //
    // ---------------------------------------------------------------------------------
    // Example (For Intrusive List):
    //
    // class MyClass : TIntrusiveLinkedList<MyClass> { ... }
    // class 
    // MyClass myList = null;              // pointer to the list
    // 
    // MyClass link = new MyClass();
    // link->LinkHead(myList);
    // ---------------------------------------------------------------------------------
    public void LinkHead(ref TLinkedListBase<ElementType> head)
    {


        if (head != null)
            head.PrevLink = NextLink;

        NextLink = head;
        PrevLink = head;
        head = this;
    }

    public bool IsLinked()
    {
        return PrevLink != null;
    }

    public TLinkedListBase<ElementType> GetPrevLink() 
	{
		return PrevLink;
	}

    public TLinkedListBase<ElementType> GetNextLink() 
	{
		return NextLink;
	}

    #region IEnumerable<T> Members

    public IEnumerator<ElementType> GetEnumerator()
    {
        DoubleLinkBase<ElementType> node = this;
        while (node != null && !node.IsRoot)
        {
            yield return node.Data;
            node = node.Next;
        }
    }

    public IEnumerator<ElementType> GetEnumeratorReversed()
    {
        DoubleLinkBase<ElementType> node = this;
        while (node != null && !node.IsRoot)
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

    private TLinkedListBase<ElementType> NextLink;
    private TLinkedListBase<ElementType> PrevLink;

};


public class TLinkedList<ElementType> : TLinkedListBase<ElementType> where ElementType : class
{
    public TLinkedList()
    {
    }

    TLinkedList(ElementType InElement) : base()
    {
        Element = InElement;
    }

    // Accessors.
    public ElementType Data() 
	{
		return Element;
	}

	private ElementType Element;
};

// Intrusive version of the double linked list
//
// Usage:
//
// class MyClass : TIntrusiveLinkedList<MyClass>() { ... }
//
// MyClass myObject1 = new MyClass();
// MyClass myObject2 = new MyClass();
// myObject1 = new MyClass();

class TIntrusiveLinkedList<ElementType> : TLinkedListBase<ElementType> where ElementType : class
{
    public TIntrusiveLinkedList()
		: base()
    {
    }
};
*/