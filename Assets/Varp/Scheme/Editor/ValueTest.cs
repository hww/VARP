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


namespace SchemeUnit
{
    using VARP.Scheme.Stx;
    using VARP.Scheme.Data;
    using VARP.Scheme.REPL;
    using NUnit.Framework;

    /// <summary>
    /// Some tests for Value class
    /// </summary>
    public class ValueTest
    {

        [Test]
        public void Boolean()
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
        public void Int()
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
        public void Double()
        {
            Value a1 = new Value(1.1);
            Value a2 = new Value(2.1);
            Value b1 = new Value(1.1);
            Value b2 = new Value(2.1);
            Assert.AreEqual(1.1, (int)a1);
            Assert.AreEqual(2.1, (int)a2);
            Assert.AreEqual(a1, b1);
            Assert.AreEqual(a2, b2);
            Assert.AreNotEqual(a1, a2);
            Assert.AreNotEqual(b1, b2);
            Assert.AreNotEqual(1.1, (int)a2);
            Assert.AreNotEqual(2.1, (int)a1);
        }

    }
}
