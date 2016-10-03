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
    public class AstEnvironment : ValueClass, IEnumerable<AstBinding>
    {
        // Pointer to the parent environment.
        // For global environment it is null
        public AstEnvironment Parent;

        // List of binding in order of adding to environment
        private List<AstBinding> Bindings = new List<AstBinding>();

        // Symbol to binding conversion table
        private Dictionary<Symbol, AstBinding> BindingsMap = new Dictionary<Symbol, AstBinding>();

        public byte Index;          //< index of this environment
        public bool isExpanded;     //< was the environment expanded    
        public int UpValuesCount;   //< how many up values

        // Create new environment and parent it to given
        public AstEnvironment(AstEnvironment parent = null)
        {
            Index = (byte)(parent == null ? 0 : parent.Index + 1);
            Parent = parent;
            UpValuesCount = 0;
            Bindings = new List<AstBinding>();
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
        public AstBinding LookupLocal(Symbol name)
        {
            AstBinding value;
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
        public AstBinding Lookup(Symbol name)
        {
            AstBinding value;
            if (BindingsMap.TryGetValue(name, out value))
                return value;
            return Parent == null ? null : Parent.Lookup(name);
        }

        /// <summary>
        /// Get definition by index. Use only for local environment
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AstBinding this[int index]
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
        public AstBinding Define(AstBinding value)
        {
            if (BindingsMap.ContainsKey(value.Identifier))
                SchemeError.Error("define", "environment already have key", value.Id);
            BindingsMap[value.Identifier] = value;
            if (Bindings.Count > 255) SchemeError.Error("define", "too many variables in frame", value.Id);
            value.VarIdx = (byte)Bindings.Count;
            Bindings.Add(value);
            return value;
        }

        /// <summary>
        /// Define primitive. The primitive is sort of macro which is
        /// implemented in C# and extract given expression to AST.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="primitive">primitive</param>
        /// <returns>new binding</returns>
        public AstBinding DefinePrimitive(string name, PrimitiveBinding.CompilerPrimitive primitive)
        {
            Debug.Assert(IsGlobal); // can be defined only in global 
            Symbol sym = Symbol.Intern(name);
            Syntax syntax = new Syntax(sym);
            AstBinding binding = new PrimitiveBinding(this, syntax, primitive);
            Define(binding);
            return binding;
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
            AstEnvironment env = Parent;
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
        public AstEnvironment GetEnvironmentAtIndex(int index)
        {
            AstEnvironment env = this;
            while (index>0 && env.Parent != null)
                env = env.Parent;
            return env;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<AstBinding> GetEnumerator()
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
        public AstBinding[] ToArray() { return Bindings.ToArray(); }

        #endregion
    }

}
