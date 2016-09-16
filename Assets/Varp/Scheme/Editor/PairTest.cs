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
using NUnit.Framework;
using VARP.Scheme.Stx;
using VARP.Scheme.Data;
using VARP.Scheme.REPL;

namespace SchemeUnit
{
	/// <summary>
	/// Some tests for Pairs
	/// </summary>
	public class ListTest
    {

        //Interpreter terp = new Interpreter();
        //Parser terp = new Parser();
        ValueList ParseScheme(string expression)
        {
            return AstBuilder.Expand(expression, "PairTes.cs").GetDatum().AsValueList();
        }


        [Test]
        public void Reverse()
        {
            ValueList list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            ValueList list2 = list1.DuplicateReverse(0,-1) as ValueList;

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(size1, size2);

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[size1 - x - 1]);
        }
        [Test]
        public void Duplicate()
        {
            ValueList list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            ValueList list2 = list1.Duplicate(0, -1) as ValueList;

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(size1, size2);
            Assert.AreEqual(10, list2[0]);

            for (int x = 0; x < size1; x++)
                Assert.AreNotEqual((list1.GetNodeAtIndex(x) as object), (list2.GetNodeAtIndex(x).GetHashCode() as object));

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[x]);
        }

        [Test]
        public void Sublist()
        {
            ValueList list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            ValueList list2 = list1.Duplicate(1, 3) as ValueList;

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(3, size2);
            Assert.AreEqual(Inspector.Inspect(list2), "(2 3 4)");


            list2 = list1.DuplicateReverse(1, 3) as ValueList; 

            Assert.AreEqual(3, size2);
            Assert.AreEqual(Inspector.Inspect(list2), "(4 3 2)");

        }

    }
}
