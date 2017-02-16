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
    using REPL;
    using System.Text;

    public sealed class Binding
    {
        public Environment environment;
        public Value value;
    }

    public sealed class Environment : ValueClass, IEnumerable<Binding>
    {
        // Create system environment
        public static Environment Top = new Environment(null, Symbol.Intern("*SYSTEM-ENV*"), 1000);

        public Environment parent;        //< pointer to parent frame
        public Symbol name;               //< environment name

        // Binding in this environment
        private Dictionary<Symbol, Binding> Bindings = new Dictionary<Symbol, Binding>();

        public Environment(Environment parent, Symbol name, int size)
        {
            this.parent = parent;
            this.name = name;
            Bindings = new Dictionary<Symbol, Binding>(size);
        }

        public bool IsTop { get { return parent == null; } }

        /// <summary>
        /// Get definition by index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Value this[Symbol name]
        {
            get { return Bindings[name].value; }
            set { Bindings[name].value = value; }
        }

        /// <summary>
        /// Safely return existing value. Does not produce 
        /// exception if value is not exists.
        /// Find only in this environment
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        public Binding LookupLocal(Symbol name)
        {
            Binding value;
            if (Bindings.TryGetValue(name, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Safely return existing value. Does not produce 
        /// exception if value is not exists
        /// Find in this environment, then try parent one
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        public Binding LookupRecursively(Symbol name)
        {
            Binding value;
            if (Bindings.TryGetValue(name, out value))
                return value;
            return parent == null ? null : parent.LookupRecursively(name);
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
            return GetEnumerator();
        }

        #endregion

        #region Value Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<lexical-environment size={0}>", Bindings.Count); }

        #endregion
        public override string Inspect() {
            var sb = new StringBuilder();
            foreach (var v in Bindings)
                sb.AppendLine( Inspector.Inspect(v.Value));
            return sb.ToString();
        }
    }
}