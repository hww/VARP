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
    using Data;


    public sealed class LexicalBinding : SObject
    {
        public delegate AST CompilerPrimitive(Syntax expression, LexicalEnvironment context);

        public Symbol Identifier;               //< variable index in the environment
        public Symbol UID;                      //< unique id of variable name.number
        public int Index;                       //< unique id of variable name.number
        public CompilerPrimitive Primitive;     //< in case if primitive

        // define global variable
        public LexicalBinding(Symbol variable, CompilerPrimitive primitive = null)
        {
            Debug.Assert(variable != null);
            this.Identifier = variable;
            this.UID = null;
            this.Index = -1; // global!
            this.Primitive = primitive;
        }
        public LexicalBinding(Symbol variable, int index, CompilerPrimitive primitive = null)
        {
            Debug.Assert(variable != null);
            Debug.Assert(index >= 0);
            this.Identifier = variable;
            this.UID = null;
            this.Index = index; // local!
            this.Primitive = primitive;
        }

        public bool IsPrimitive { get { return Primitive != null; } }
        public bool IsGlobal { get { return Index < 0; } }

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string AsString() { return base.ToString(); }
        #endregion
    }

    public class LexicalEnvironment : SObject, IEnumerable<LexicalBinding>
    {
        // Pointer to the parent environment.
        // For global environment it is null
        public LexicalEnvironment Parent;
        // List of binding in order of adding to environment
        List<LexicalBinding> Bindings = new List<LexicalBinding>();
        // Symbol to binding conversion table
        Dictionary<Symbol, LexicalBinding> BindingsMap = new Dictionary<Symbol, LexicalBinding>();
        // List of child environments. Required for inspection
        public List<LexicalEnvironment> Children = new List<LexicalEnvironment>();

        public LexicalEnvironment(LexicalEnvironment parent = null)
        {
            Parent = parent;
            Bindings = new List<LexicalBinding>();
            if (parent != null) parent.ChildAdd(this);
        }

        #region Child Environments
        public void ChildAdd(LexicalEnvironment child) { Children.Add(child); }
        public void ChildRemove(LexicalEnvironment child) { Children.Remove(child); }
        #endregion

        #region LexicalEnvironment Methods
        public bool IsGlobal { get { return Parent == null; } }
        public bool IsDefined(Symbol name) { return BindingsMap.ContainsKey(name); }
        public bool IsDefined(string name) { return BindingsMap.ContainsKey(Symbol.Intern(name)); }

        public LexicalBinding Lookup(Symbol name)
        {
            LexicalBinding value;
            if (BindingsMap.TryGetValue(name, out value))
                return value;
            return null;
        }
        public LexicalBinding Lookup(string name)
        {
            return Lookup(Symbol.Intern(name));
        }

        public LexicalBinding this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < Bindings.Count);
                return Bindings[index];
            }
        }
        public void Define(Symbol name, LexicalBinding value)
        {
            BindingsMap[name] = value;
            Bindings.Add(value);
        }

        public void Define(string name, LexicalBinding value)
        {
            Define(Symbol.Intern(name), value);
        }

        public LexicalBinding DefinePrimitive(Symbol name, LexicalBinding.CompilerPrimitive primitive)
        {
            LexicalBinding binding = new LexicalBinding(name, primitive);
            Define(name, binding);
            return binding;
        }
        public LexicalBinding DefinePrimitive(string name, LexicalBinding.CompilerPrimitive primitive) { return DefinePrimitive(Symbol.Intern(name), primitive);  }
        public LexicalBinding DefineVariable(Symbol name)
        {
            LexicalBinding binding = new LexicalBinding(name, Bindings.Count);
            Define(name, binding);
            return binding;
        }
        public LexicalBinding DefineVariable(string name) { return DefineVariable(Symbol.Intern(name)); }

        public int GetLevelNumber()
        {
            int level = 0;
            LexicalEnvironment p = Parent;
            while (p != null)
            {
                level++;
                p = p.Parent;
            }
            return level;
        }
        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<LexicalBinding> GetEnumerator()
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

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string ToString() { return string.Format("#<lexical-environment size={0}>", Bindings.Count); }
        public override string AsString() { return base.ToString(); }

        #endregion
    }

}
