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

        private List<AST> Initializers = new List<AST>();

        public bool isExpanded;
        public int Requried;
        public int Optional;
        public int Keys;
        public int Rest;

        // Create new environment and parent it to given
        public Environment(Environment parent = null)
        {
            Parent = parent;
            Bindings = new List<Binding>();
        }

        /// <summary>
        /// Create new child environment
        /// </summary>
        /// <param name="expression">Expression where is this arguments list</param>
        /// <param name="arguments">Arguments list</param>
        /// <returns>new environment</returns>
        public Environment CreateEnvironment(Syntax expression, Arguments arguments)
        {
            Environment env = new Environment(this);
            env.ExpandLocals(expression, arguments);
            return env;
        }

        /// <summary>
        /// Expand environment to and define list of given arguments
        /// </summary>
        /// <param name="expression">Expression where is this arguments list</param>
        /// <param name="arguments"></param>
        public void ExpandLocals(Syntax expression, Arguments arguments)
        {
            if (arguments.required != null)
                Requried = ExpandRequired(expression, arguments.required);
            if (arguments.optional != null)
                Optional = ExpandOptional(expression, arguments.optional);
            if (arguments.key != null)
                Keys = ExpandOptional(expression, arguments.key);
            if (arguments.rest != null)
            {
                Syntax synt = arguments.rest[1].AsSyntax();
                DefineVariable(synt.AsIdentifier());
                Rest = 1;
            }
        }
        private int ExpandRequired(Syntax expression, ValueList arguments)
        {
            int count = 0;
            foreach (var arg in arguments)
            {
                Syntax astx = arg.AsSyntax();
                if (astx != null && astx.IsIdentifier)
                {
                    DefineVariable(astx.AsIdentifier());
                    count++;
                }
                else
                    throw SchemeError.ArgumentError("lambda", "identifier?", count, arguments);
            }
            return count;
        }
        private int ExpandOptional(Syntax expression, ValueList arguments)
        {
            int count = 0;
            foreach (var arg in arguments)
            {
                if (arg.IsSyntax)
                {
                    Syntax varname = arg.AsSyntax();
                    if (varname.IsIdentifier)
                    {
                        Symbol ident = varname.AsIdentifier();
                        if (ident == Symbol.OPTIONAL) continue;
                        if (ident == Symbol.KEY) continue;
                        DefineVariable(ident);
                    }
                    else
                        throw SchemeError.ArgumentError("lambda", "identifier?", count, arguments);
                }
                else if (arg.IsValueList)
                {
                    ValueList p = arg.AsValueList();
                    object v0 = p[0].RefVal;
                    object v1 = p[1].RefVal;
                    Symbol ident = null;
                    AST value = null;

                    if (v0 is Syntax && (v0 as Syntax).IsIdentifier)
                        ident = (v0 as Syntax).AsIdentifier();
                    else
                        throw SchemeError.ArgumentError("lambda", "syntax?", count, arguments);

                    if (v1 is AST)
                        value = v1 as AST;
                    else
                        throw SchemeError.ArgumentError("lambda", "ast?", count, arguments);

                    Syntax var = arg.AsSyntax();

                    DefineVariable(ident);
                }
                count++;
            }
            return count;
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
        public void Define(Symbol name, Binding value)
        {
            BindingsMap[name] = value;
            Bindings.Add(value);
        }

        /// <summary>
        /// Define primitive. The primitive is sort of macro which is
        /// implemented in C# and extract given expression to AST.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="primitive">primitive</param>
        /// <returns>new binding</returns>
        public Binding DefinePrimitive(Symbol name, Binding.CompilerPrimitive primitive)
        {
            Binding binding = new Binding(name, primitive);
            Define(name, binding);
            return binding;
        }
        /// <summary>
        /// Define primitive. The primitive is sort of macro which is
        /// implemented in C# and extract given expression to AST.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="primitive">primitive</param>
        /// <returns>new binding</returns>
        public Binding DefinePrimitive(string name, Binding.CompilerPrimitive primitive) 
        {
            return DefinePrimitive(Symbol.Intern(name), primitive);
        }

        /// <summary>
        /// Define variable with given name 
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>new binding</returns>
        public Binding DefineVariable(Symbol name)
        {
            Binding binding = new Binding(name, Bindings.Count);
            Define(name, binding);
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
            Requried = 0;
            Optional = 0;
            Keys = 0;
            Rest = 0;
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
        public override string AsString() { return base.ToString(); }

        #endregion
    }

}
