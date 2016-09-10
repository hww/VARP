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

 using System.Collections.Generic;

namespace VARP.Scheme.VM
{
    using Data;

    public struct Binding
    {
        public Environment environment;
        public SObject value;
    }

    public sealed class Environment : SObject, IEnumerable<Binding>
    {
        public Environment parent;        //< pointer to parent frame
        public Symbol name;               //< environment name

        Dictionary<Symbol, Binding> Bindings = new Dictionary<Symbol, Binding>();

        public Environment(Environment parent, Symbol name, int size)
        {
            this.parent = parent;
            this.name = name;
            this.Bindings = new Dictionary<Symbol, Binding>(size);
        }

        #region IEnumerable<T> Members

        public IEnumerator<Binding> GetEnumerator()
        {
            foreach (var b in Bindings.Values)
                yield return b;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }

        #endregion

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string ToString() { return string.Format("#<lexical-environment size={0}>", Bindings.Count); }
        public override string AsString() { return base.ToString(); }

        #endregion

    }
}