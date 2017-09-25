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
using NUnit.Framework;
/*
namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using REPL;
    using VARP.Scheme.VM;

    /// <summary>
    /// The lexical environment
    /// </summary>
    public sealed partial class Environment : SObject, IEnumerable<Binding>
    {





        #region Environment Methods

        /// <summary>
        /// Returns true if this environment is global
        /// </summary>
        public bool IsGlobal { get { return FrameNum == 0; } }

        /// <summary>
        /// Safely return existing binding. Does not produce 
        /// exception if binding is not exists.
        /// Find only in this environment
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        public int IndexOfLocalVariable(Symbol name)
        {
            for (var i = 0; i < Bindings.Count; i++)
            {
                if (Bindings[i].Id.GetDatum() == name)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal virtual Environment LookupEnvironment(Symbol name, ref int frameIdx, ref int varIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var curVarIndex = curframe.IndexOfLocalVariable(name);
                if (curVarIndex >= 0)
                {
                    varIdx = curVarIndex;
                    frameIdx = FrameNum - curframe.FrameNum;
                    return curframe;
                }
                curframe = curframe.Parent;
            }

            return null;
        }

        internal virtual AstBinding Lookup(Symbol name, ref int frameIdx, ref int varIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var curVarIndex = curframe.IndexOfLocalVariable(name);
                if (curVarIndex >= 0)
                {
                    varIdx = curVarIndex;
                    frameIdx = FrameNum - curframe.FrameNum;
                    return curframe[varIdx];
                }
                curframe = curframe.Parent;
            }

            return null;
        }
        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal AstBinding Lookup(Symbol name)
        {
            var valueIdx = IndexOfLocalVariable(name);

            if (valueIdx >= 0)
                return Bindings[valueIdx];

            return null;
        }

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal AstBinding LockupRecursively(Symbol name)
        {
            int frameIdx = 0;
            int valueIdx = 0;
            var env = LookupEnvironment(name, ref frameIdx, ref valueIdx);

            if (env != null)
                return env[valueIdx];

            return null;
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
        /// <param name="binding">binding</param>
        public int Define(AstBinding binding)
        {
            var idx = IndexOfLocalVariable(binding.Identifier);
            if (idx >= 0)
                throw SchemeError.Error("define", "environment already have key", binding.Id);

            binding.VarIdx = Bindings.Count;
            Bindings.Add(binding);
            return binding.VarIdx;
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
            var sym = Symbol.Intern(name);
            var syntax = new Syntax(sym);
            var binding = new PrimitiveBinding(syntax, primitive);
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
            Parent = null;
        }

        /// <summary>
        /// Return level of this environment is the stack
        /// </summary>
        /// <returns>0 for root (global or system) environment</returns>
        public int GetEnvironmentIndex()
        {
            var index = 0;
            var env = Parent;
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
            var env = this;
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
            return GetEnumerator();
        }

        #endregion

        #region ValueType Methods

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<lexical-environment size={0}>", Bindings.Count); }
        public AstBinding[] ToArray() { return Bindings.ToArray(); }

        #endregion
    }

}
*/