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

namespace SchemeUnit
{
    using VARP.Scheme.Stx;
    using VARP.Scheme.Data;
    using VARP.Scheme.REPL;


    /// <summary>
    /// Some tests for Value class
    /// </summary>
    public class ValueTest
    {

        [Test]
        public void BooleanTest()
        {
            Value t = new Value(true);
            Value f = new Value(false);
            Value t1 = new Value(true);
            Value f1 = new Value(false);
            Assert.AreEqual(true, (bool)t);
            Assert.AreEqual(false, (bool)f);
            Assert.AreEqual(t, t1);
            Assert.AreEqual(f, f1);
            Assert.AreNotEqual(t, f);
            Assert.AreNotEqual(t1, f1);
            Assert.AreNotEqual(true, (bool)f);
            Assert.AreNotEqual(false, (bool)t);
        }

        [Test]
        public void CharTest()
        {
            Value t = new Value('A');
            Value f = new Value('B');
            Value t1 = new Value('A');
            Value f1 = new Value('B');
            Assert.AreEqual('A', (char)t);
            Assert.AreEqual('B', (char)f);
            Assert.AreEqual(t, t1);
            Assert.AreEqual(f, f1);
            Assert.AreNotEqual(t, f);
            Assert.AreNotEqual(t1, f1);
            Assert.AreNotEqual('A', (char)f);
            Assert.AreNotEqual('B', (char)t);
        }

        [Test]
        public void IntTest()
        {
            Value a1 = new Value(1);
            Value a2 = new Value(2);
            Value b1 = new Value(1);
            Value b2 = new Value(2);
            Assert.AreEqual(1, (int)a1);
            Assert.AreEqual(2, (int)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(1, (int)a2);
            Assert.AreNotEqual(2, (int)a1);
        }

        [Test]
        public void DoubleTest()
        {
            Value a1 = new Value(1.1);
            Value a2 = new Value(2.1);
            Value b1 = new Value(1.1);
            Value b2 = new Value(2.1);
            Assert.AreEqual(1.1, (double)a1);
            Assert.AreEqual(2.1, (double)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(1.1, (double)a2);
            Assert.AreNotEqual(2.1, (double)a1);
        }

        [Test]
        public void StringTest()
        {
            Value a1 = new Value("x1");
            Value a2 = new Value("x2");
            Value b1 = new Value("x1");
            Value b2 = new Value("x2");
            Assert.AreEqual("x1", (string)a1);
            Assert.AreEqual("x2", (string)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual("x1", (string)a2);
            Assert.AreNotEqual("x2", (string)a1);
        }

        [Test]
        public void SymbolTest()
        {
            Value a1 = new Value(Symbol.Intern("x1"));
            Value a2 = new Value(Symbol.Intern("x2"));
            Value b1 = new Value(Symbol.Intern("x1"));
            Value b2 = new Value(Symbol.Intern("x2"));
            Assert.AreEqual(Symbol.Intern("x1"), (Symbol)a1);
            Assert.AreEqual(Symbol.Intern("x2"), (Symbol)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(Symbol.Intern("x1"), (Symbol)a2);
            Assert.AreNotEqual(Symbol.Intern("x2"), (Symbol)a1);
        }

        [Test]
        public void ValueListTest()
        {
            ValueList x1 = ValueList.ListFromArguments(1, 2, 3);
            ValueList x2 = ValueList.ListFromArguments(4, 5, 6);
            Value a1 = new Value(x1);
            Value a2 = new Value(x2);
            Value b1 = new Value(x1);
            Value b2 = new Value(x2);
            Assert.AreEqual(x1, (ValueList)a1);
            Assert.AreEqual(x2, (ValueList)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(x1, (ValueList)a2);
            Assert.AreNotEqual(x2, (ValueList)a1);
        }

        [Test]
        public void ValueVectorTest()
        {
            ValueVector x1 = ValueVector.VectorFromArguments(1, 2, 3);
            ValueVector x2 = ValueVector.VectorFromArguments(4, 5, 6);
            Value a1 = new Value(x1);
            Value a2 = new Value(x2);
            Value b1 = new Value(x1);
            Value b2 = new Value(x2);
            Assert.AreEqual(x1, (ValueVector)a1);
            Assert.AreEqual(x2, (ValueVector)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(x1, (ValueVector)a2);
            Assert.AreNotEqual(x2, (ValueVector)a1);
        }
        [Test]
        public void ValueTableTest()
        {
            Symbol a1 = Symbol.Intern("a1");
            Symbol a2 = Symbol.Intern("a2");
            Symbol b1 = Symbol.Intern("b1");
            Symbol b2 = Symbol.Intern("b2");

            ValueTable table1 = ValueTable.TableFromArguments(a1, 1, a2, 2, b1, 1, b2, 2);
            ValueTable table2 = ValueTable.TableFromArguments(a1, 1, a2, 2, b1, 1, b2, 2);
            ValueTable table3 = ValueTable.TableFromArguments(a1, 0, a2, 2, b1, 1, b2, 2);

            Assert.AreEqual(1, (int)table1[a1]);
            Assert.AreEqual(2, (int)table1[a2]);
            Assert.AreEqual(table1[a1], table1[b1]);
            Assert.AreEqual(table1[a2], table1[b2]);
            Assert.AreNotEqual(2, (int)table1[a1]);
            Assert.AreNotEqual(1, (int)table1[a2]);
            Assert.AreNotEqual(table1[a1], table1[b2]);
            Assert.AreNotEqual(table1[a2], table1[a1]);

            Assert.AreEqual(table1, table2);
            Assert.AreNotEqual(table1, table3);
        }

    }
}
