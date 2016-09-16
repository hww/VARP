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

namespace SchemeUnit
{
	/// <summary>
	/// Some tests for Pairs
	/// </summary>
	public class ParserTest
    {

        //Interpreter terp = new Interpreter();
        //Parser terp = new Parser();
        Pair ParseScheme(string expression)
        {
            return AstBuilder.Expand(expression, "PairTes.cs").GetDatum().ToPair();
        }

		[Test]
		public void TestLoop()
		{
			Pair shortList = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
			Pair lastElement = shortList;
			Pair loopElement = shortList;

			Assert.False(shortList.HasLoop());

			for (int x=0; x<9; x++) lastElement = (Pair)lastElement.Cdr;
			for (int x=0; x<3; x++) loopElement = (Pair)loopElement.Cdr;

			// Make a loop
			lastElement.Cdr = loopElement;

			Assert.True(shortList.HasLoop());
		}

		[Test]
		public void TestImproper()
		{
			Pair shortList = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");

			Assert.False(shortList.IsImproper());

			Pair improperList = ParseScheme("(1 2 3 4 5 6 7 8 9 . 10)");

			Assert.True(improperList.IsImproper());
		}

		[Test]
		public void TestImproperWithLoop()
		{
			Pair shortList = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
			Pair lastElement = shortList;
			Pair loopElement = shortList;

			for (int x=0; x<9; x++) lastElement = (Pair)lastElement.Cdr;
			for (int x=0; x<3; x++) loopElement = (Pair)loopElement.Cdr;

			// Make a loop
			lastElement.Cdr = loopElement;

			Assert.False(shortList.IsImproper());
		}

		[Test]
		public void TestLoopOrImproper()
		{
			Pair shortList = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
			Pair lastElement = shortList;
			Pair loopElement = shortList;

			Assert.False(shortList.IsImproperOrLoop());
			
			Pair improperList = ParseScheme("(1 2 3 4 5 6 7 8 9 . 10)");
			Assert.True(improperList.IsImproperOrLoop());

			for (int x=0; x<9; x++) lastElement = (Pair)lastElement.Cdr;
			for (int x=0; x<3; x++) loopElement = (Pair)loopElement.Cdr;

			// Make a loop
			lastElement.Cdr = loopElement;

			Assert.True(shortList.IsImproperOrLoop());
		}

		[Test]
		public void FindLoopHead()
		{
			Pair shortList = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
			Pair lastElement = shortList;
			Pair loopElement = shortList;

            Assert.AreEqual(null, shortList.GetLoopHead());

			for (int x=0; x<9; x++) lastElement = lastElement.Cdr.ToPair();
			for (int x=0; x<3; x++) loopElement = loopElement.Cdr.ToPair();

			Assert.AreEqual(10, lastElement.Car);
			Assert.AreEqual(4, loopElement.Car);

			// Make a loop
			lastElement.Cdr = loopElement;

			Assert.AreEqual(4, shortList.GetLoopHead().Car);
			Assert.AreEqual(loopElement, shortList.GetLoopHead());
		}

        [Test]
        public void Reverse()
        {
            Pair list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            Pair list2 = Pair.Reverse(list1);

            int size1 = Pair.Length(list1);
            int size2 = Pair.Length(list2);

            Assert.AreEqual(size1, size2);

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[size1 - x - 1]);
        }
        [Test]
        public void Duplicate()
        {
            Pair list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            Pair list2 = null;
            Pair last = null;
            int size0 = Pair.Duplicate(list1, ref list2, ref last);

            int size1 = Pair.Length(list1);
            int size2 = Pair.Length(list2);

            Assert.AreEqual(size0, size1);
            Assert.AreEqual(size1, size2);
            Assert.AreEqual(10, last.Car);

            for (int x = 0; x < size1; x++)
                Assert.AreNotEqual((list1.PairAtIndex(x) as object), (list2.PairAtIndex(x).GetHashCode() as object));

            for (int x = 0; x < size1; x++)
                Assert.AreEqual(list1[x], list2[x]);
        }

        [Test]
        public void Sublist()
        {
            Pair list1 = ParseScheme("(1 2 3 4 5 6 7 8 9 10)");
            Pair list2 = null;
            Pair last = null;
            int res_size = Pair.Sublist(list1, 1, 3, ref list2, ref last);

            int size2 = Pair.Length(list2);

            Assert.AreEqual(res_size, size2);
            Assert.AreEqual(list2.AsString(), "(2 3 4)");


            res_size = Pair.Sublist(list1, -4, -2, ref list2, ref last);

            size2 = Pair.Length(list2);

            Assert.AreEqual(res_size, size2);
            Assert.AreEqual(list2.AsString(), "(7 8 9)");

        }

    }
}
