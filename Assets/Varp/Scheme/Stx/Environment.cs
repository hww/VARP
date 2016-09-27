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
using System.Diagnostics;

namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using REPL;

    /// <summary>
    /// The lexical environment
    /// </summary>
    public class Environment : ValueClass, IEnumerable<Binding>
    {
        // Pointer to the parent environment.
        // For global environment it is null
        public Environment Parent;

        // List of binding in order of adding to environment
        private List<Binding> Bindings = new List<Binding>();

        // Symbol to binding conversion table
        private Dictionary<Symbol, Binding> BindingsMap = new Dictionary<Symbol, Binding>();

        public int Index;           //< index of this environment
        public bool isExpanded;     //< was the environment expanded    
        public int UpValuesCount;   //< how many up values

        // Create new environment and parent it to given
        public Environment(Environment parent = null)
        {
            Index = parent == null ? 0 : parent.Index + 1;
            Parent = parent;
            UpValuesCount = 0;
            Bindings = new List<Binding>();
        }



        #region Environment Methods

        /// <summary>
        /// Returns true if this environment is global
        /// </summary>
        public bool IsGlobal { get { return Parent == null; } }

        /// <summary>
        /// Test if this definition already exists
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns></returns>
        public bool IsDefined(string name) { return BindingsMap.ContainsKey(Symbol.Intern(name)); }

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
            if (BindingsMap.TryGetValue(name, out value))
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
        public Binding Lookup(Symbol name)
        {
            Binding value;
            if (BindingsMap.TryGetValue(name, out value))
                return value;
            return Parent == null ? null : Parent.Lookup(name);
        }

        /// <summary>
        /// Get definition by index. Use only for local environment
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Binding this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < Bindings.Count);
                return Bindings[index];
            }
        }

        /// <summary>
        /// Define new definition.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="value">binding</param>
        public void Define(Binding value)
        {
            BindingsMap[value.Identifier] = value;
            Debug.Assert(Bindings.Count < 256);
            value.Index = (byte)Bindings.Count;
            Bindings.Add(value);
        }

        /// <summary>
        /// Define primitive. The primitive is sort of macro which is
        /// implemented in C# and extract given expression to AST.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="primitive">primitive</param>
        /// <returns>new binding</returns>
        public Binding DefinePrimitive(Symbol name, PrimitiveBinding.CompilerPrimitive primitive)
        {
            Debug.Assert(IsGlobal); // can be defined only in global 
            Syntax syntax = new Syntax(name);
            Binding binding = new PrimitiveBinding(this, syntax, primitive);
            Define(binding);
            return binding;
        }
        /// <summary>
        /// Define primitive. The primitive is sort of macro which is
        /// implemented in C# and extract given expression to AST.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="primitive">primitive</param>
        /// <returns>new binding</returns>
        public Binding DefinePrimitive(string name, PrimitiveBinding.CompilerPrimitive primitive) 
        {
            return DefinePrimitive(Symbol.Intern(name), primitive);
        }

        /// <summary>
        /// Clear environment
        /// </summary>
        public void Clear()
        {
            Bindings.Clear();
            Bindings = null;
            BindingsMap.Clear();
            BindingsMap = null;
            Parent = null;
            UpValuesCount = 0;
        }

        /// <summary>
        /// Return level of this environment is the stack
        /// </summary>
        /// <returns>0 for root (global or system) environment</returns>
        public int GetEnvironmentIndex()
        {
            int index = 0;
            Environment env = Parent;
            while (env != null) {
                env = env.Parent; index++;
            }
            return index;
        }

        /// <summary>
        /// Returns environment with @index levels up or global
        /// </summary>
        /// <param name="index">index of the enviromnet</param>
        /// <returns>environment</returns>
        public Environment GetEnvironmentAtIndex(int index)
        {
            Environment env = this;
            while (index>0 && env.Parent != null)
                env = env.Parent;
            return env;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<Binding> GetEnumerator()
        {
            foreach (var b in Bindings)
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

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<lexical-environment size={0}>", Bindings.Count); }
        public Binding[] ToArray() { return Bindings.ToArray(); }

        #endregion
    }

}
