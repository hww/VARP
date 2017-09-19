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
using System.Collections.Generic;

namespace VARP.Scheme.Test
{
    using VARP.Scheme.Stx;
    using VARP.Scheme.Data;
    using VARP.Scheme.REPL;
    using VARP.DataStructures;


    /// <summary>
    /// Some tests for Value class
    /// </summary>
    public class ValueToStringTest
    {
        static string True = BoolType.True.ToString();
        static string False = BoolType.False.ToString();

        [Test]
        public void BooleanTest()
        {
            Value t = new Value(true);
            Value f = new Value(false);
            Value t1 = new Value(true);
            Value f1 = new Value(false);
            Assert.AreEqual(True, t.ToString());
            Assert.AreEqual(False, f.ToString());
        }
        
        [Test]
        public void CharTest()
        {
            Value a = new Value('A');
            Value b = new Value('B');
            Assert.AreEqual("#\\A", a.ToString());
            Assert.AreEqual("#\\B", b.ToString());
        }

        [Test]
        public void IntTest()
        {
            Value a1 = new Value(1);
            Value a2 = new Value(2);
            Assert.AreEqual("1", a1.ToString());
            Assert.AreEqual("2", a2.ToString());
        }

        [Test]
        public void DoubleTest()
        {
            Value a1 = new Value(1.1);
            Value a2 = new Value(2.1);
            Assert.AreEqual("1.1", a1.ToString());
            Assert.AreEqual("2.1", a2.ToString());
        }

        [Test]
        public void StringTest()
        {
            Value a1 = new Value("x1");
            Value a2 = new Value("x2");
            Assert.AreEqual("x1", a1.ToString());
            Assert.AreEqual("x2", a2.ToString());
        }

        [Test]
        public void SymbolTest()
        {
            Value a1 = new Value(Symbol.Intern("x1"));
            Value a2 = new Value(Symbol.Intern("x2"));
            Assert.AreEqual("x1", a1.ToString());
            Assert.AreEqual("x2", a2.ToString());
        }

        [Test]
        public void LinkedListTest()
        {
            string expect = "(1 2 3.1)";
            LinkedList<Value> x1 = ValueLinkedList.FromArguments(1, 2, 3.1);
            Value a1 = new Value(x1);
            Assert.AreEqual(expect, x1.ToString());
            Assert.AreEqual(expect, a1.ToString());
        }

        [Test]
        public void ValueListTest()
        {
            string expect = "#(1 2 3.1 \"4\")";

            List<Value> x1 = ValueList.FromArguments(1, 2, 3.1, "4");
            Value a1 = new Value(x1);
            Assert.AreEqual(expect, ValueString.ToString(x1));
            Assert.AreEqual(expect, a1.ToString());
        }

        [Test]
        public void DictionaryTest()
        {
            string expect = "#hash((a1 . 1) (a2 . 2) (\"b1\" . \"1\") (\"b2\" . 2.2))";

            Symbol a1 = Symbol.Intern("a1");
            Symbol a2 = Symbol.Intern("a2");
            string b1 = "b1";
            string b2 = "b2";

            Dictionary<object, Value> table1 = ValueDictionary.FromArguments(a1, 1, a2, 2, b1, "1", b2, 2.2);
            Value t1 = new Value(table1);
            Assert.AreEqual(expect, ValueString.ToString(table1));
            Assert.AreEqual(expect, t1.ToString());

        }

    }
}
