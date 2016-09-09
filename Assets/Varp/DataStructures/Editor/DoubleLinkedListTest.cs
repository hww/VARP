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
using UnityEditor;
using NUnit.Framework;

using VARP.DataStructures;
public class DoubleLinkedListTest {

    class MyClass
    {
        public LinkedListNode<MyClass> link;
        public int value;
        public MyClass(int val) 
        {
            value = val;
            link = new LinkedListNode<MyClass>(this);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return value.Equals((obj as MyClass).value);
        }

        public override string ToString()
        {
            return value.ToString();
        }

    };

	[Test]
	public void DoubleLinkedListTestRun()
	{
        LinkedList<string> stringsList = new LinkedList<string>();
        stringsList.AddLast(new LinkedListNode<string>("1"));
        stringsList.AddLast(new LinkedListNode<string>("2"));
        stringsList.AddFirst(new LinkedListNode<string>("0"));
        Debug.Log(stringsList.ToString());
        string[] stringArray = stringsList.ToArray();
        Debug.Assert(stringArray[0] == "0");
        Debug.Assert(stringArray[1] == "1");
        Debug.Assert(stringArray[2] == "2");
        Debug.Assert(stringsList.ToList().ToArray().ToString() == new string[] { "0", "1", "2" }.ToString());
        Debug.Assert(stringsList.ToArray().ToString() == new string[] { "0", "1", "2" }.ToString());
        string iteratorString = "";
        foreach (string v in stringsList)
        {
            iteratorString += v.ToString();
        }
        Debug.Assert(iteratorString == "012");

        LinkedList<int> intList = new LinkedList<int>();
        intList.AddLast(new LinkedListNode<int>(1));
        intList.AddLast(new LinkedListNode<int>(2));
        intList.AddFirst(new LinkedListNode<int>(0));
        Debug.Log(stringsList.ToString());
        int[] intArray = intList.ToArray();
        Debug.Assert(intArray[0] == 0);
        Debug.Assert(intArray[1] == 1);
        Debug.Assert(intArray[2] == 2);
        Debug.Assert(intList.ToList().ToArray().ToString() == new int[] { 0, 1, 2 }.ToString());
        Debug.Assert(intList.ToArray().ToString() == new int[] { 0, 1, 2 }.ToString());


        LinkedList<MyClass> intusiveList = new LinkedList<MyClass>();
        intusiveList.AddLast(new MyClass(1).link);
        intusiveList.AddLast(new MyClass(2).link);
        intusiveList.AddFirst(new MyClass(0).link);
        Debug.Log(stringsList.ToString());
        MyClass[] classArray = intusiveList.ToArray();
        Debug.Assert(classArray[0].value == 0);
        Debug.Assert(classArray[1].value == 1);
        Debug.Assert(classArray[2].value == 2);
        Debug.Assert(intusiveList.ToList().ToArray().ToString() == new MyClass[] { new MyClass(0), new MyClass(1), new MyClass(2) }.ToString());
        Debug.Assert(intusiveList.ToArray().ToString() == new MyClass[] { new MyClass(0), new MyClass(1), new MyClass(2) }.ToString());
    }
}
