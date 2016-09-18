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

using NUnit.Framework;

namespace VARP.Scheme.Test
{
    using VARP.Scheme.Stx;
    using VARP.Scheme.Data;
    using VARP.Scheme.REPL;
    using VARP.DataStructures;

    /// <summary>
    /// Some tests for Pairs
    /// </summary>
    public class ValueListTest
    {

        //Interpreter terp = new Interpreter();
        //Parser terp = new Parser();
        LinkedList<Value> ParseScheme(string expression)
        {
            AST ast = AstBuilder.Expand(expression, "PairTes.cs");
            Value datum = ast.GetDatum();

            return datum.AsLinkedList<Value>();
        }

        [Test]
        public void Reverse()
        {
            LinkedList<Value> list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            list1.Reverse();

            int size1 = list1.Count;

            Assert.AreEqual(size1, 10);

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], 10-x);
        }

        [Test]
        public void DuplicateReverse()
        {
            LinkedList<Value> list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            LinkedList<Value> list2 = list1.DuplicateReverse(0, -1);

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(size1, size2);

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[size1 - x - 1]);
        }
        [Test]
        public void Duplicate()
        {
            LinkedList<Value> list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            LinkedList<Value> list2 = list1.Duplicate(0, -1);

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(size1, size2);
            Assert.AreEqual(1, list2[0].AsInt32());

            for (int x = 0; x < size1; x++)
                Assert.AreNotSame(list1.GetNodeAtIndex(x), list2.GetNodeAtIndex(x));

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[x]);
        }

        [Test]
        public void Sublist()
        {
            LinkedList<Value> list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            LinkedList<Value> list2 = list1.Duplicate(1, 3);

            int size1 = list1.Count;
            int size2 = list2.Count;

            Assert.AreEqual(3, size2);
            Assert.AreEqual("(2 3 4)", Inspector.Inspect(list2));


            list2 = new LinkedList<Value>(list1.DuplicateReverse(1, 3));

            Assert.AreEqual(3, size2);
            Assert.AreEqual("(4 3 2)", Inspector.Inspect(list2));

        }

    }
}
