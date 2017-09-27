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
    using Exception;
    using Stx;
    using VARP.Scheme.Stx.Primitives;
    using System;

    public class Environment : SObject, IEnumerable<Binding>
    {
        //! Default capacity of new environment
        public const int DEFAULT_ENVIRONMENT_CAPACITY = 16;

        //! pointer to parent frame
        public Environment Parent;

        //! environment name
        public Symbol Name;

        //! index of this environment
        public readonly int FrameNum;       

        //! Binding in this environment
        private Dictionary<Symbol, Binding> Bindings = new Dictionary<Symbol, Binding>();

        /// <summary>
        /// Create new environment
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="capacity"></param>
        public Environment(Environment parent, Symbol name, int capacity = DEFAULT_ENVIRONMENT_CAPACITY)
        {
            this.Parent = parent;
            this.Name = name;
            this.FrameNum = parent == null ? 0 : parent.FrameNum + 1;
            this.IsLexical = parent == null ? false : parent.IsLexical;
            Bindings = new Dictionary<Symbol, Binding>(capacity);
        }

        /// <summary>
        /// Create new environment
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="capacity"></param>
        public Environment(Environment parent, Symbol name, bool isLexical, int capacity = DEFAULT_ENVIRONMENT_CAPACITY)
        {
            this.Parent = parent;
            this.Name = name;
            this.FrameNum = parent == null ? 0 : parent.FrameNum + 1;
            this.IsLexical = isLexical;
            Bindings = new Dictionary<Symbol, Binding>(capacity);
        }

        /// <summary>
        /// Check if this environment is top
        /// </summary>
        public bool IsTop { get { return Parent == null; } }

        /// <summary>
        /// Return the capacity of environment
        /// </summary>
        public int Count { get { return Bindings.Count; } } 

        /// <summary>
        /// Is this environment lexical
        /// </summary>
        public bool IsLexical { get; set; }

        /// <summary>
        /// Get definition by index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Value this[Symbol name]
        {
            get
            {
                Binding bind = null;
                if (Bindings.TryGetValue(name, out bind))
                    return Bindings[name].value;
                else
                    return Value.Nil;
            }
            set
            {
                Binding bind = null;
                if (Bindings.TryGetValue(name, out bind))
                    bind.value = value;
                else
                    Bindings[name] = new Binding() {environment = this, value = value }; 
            }
        }

        /// <summary>
        /// Update or define value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Binding DefineOrUpdate(Symbol name, Value value)
        {
            Binding bind = null;
            if (Bindings.TryGetValue(name, out bind))
            {
                bind.value = value;
            }
            else
            {
                Bindings[name] = bind = new Binding() { environment = this, value = value };
            }
            return bind;
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
            return Parent == null ? null : Parent.LookupRecursively(name);
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
        public override string ToString() { return string.Format("#<environment count={0}>", Bindings.Count); }

        /// <summary>
        /// Convert environment to array of bindings
        /// </summary>
        /// <returns></returns>
        public AstBinding[] ToArray()
        {
            var array = new AstBinding[Bindings.Count];
            Bindings.Values.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Convert ast environment to the ast array
        /// </summary>
        /// <returns></returns>
        public AstBinding[] ToAstArray() {
            if (IsLexical)
            {
                var array = new AstBinding[Bindings.Count];
                foreach (var bind in Bindings)
                {
                    if (bind.Value is AstBinding)
                    {
                        var ast = bind.Value as AstBinding;
                        array[ast.VarIdx] = ast;
                    }
                    else throw new Exception();
                }
                return array;
            }
            else throw new Exception("Excepted lexical array");
        }

        #endregion

        #region Debuggin and Inspection

        public override string Inspect() {
            var sb = new StringBuilder();
            foreach (var v in Bindings)
                sb.AppendLine( Inspector.Inspect(v.Value));
            return sb.ToString();
        }

        #endregion

        #region Factory

        public static Environment Create(Environment parent, Symbol name, Frame dynFrame, int capacity = DEFAULT_ENVIRONMENT_CAPACITY)
        {
            Debug.Assert(dynFrame != null);

            var frames = dynFrame.ToArray();

            foreach (var frame in frames)
            {
                var template = frame.template;
                var varcount = template.Variables.Length;

                parent = new Environment(parent, Symbol.NULL, true);

                for (var i = 0; i < varcount; i++)
                {
                    var variable = template.GetVariable(i);
                    var syntax = new Syntax(template.Variables[i].Name);

                    switch (variable.Type)
                    {
                        case VariableType.Global:
                            parent.Define(new GlobalBinding(syntax));
                            break;

                        case VariableType.Local:
                            parent.Define(new LocalBinding(syntax));
                            break;

                        case VariableType.UpValue:
                            parent.Define(new UpBinding(syntax, variable.UpEnvIdx, variable.UpVarIndex));
                            break;
                    }
                }
            }
            return parent;
        }


        #endregion


        #region AST Bindings

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal Environment LookupAstEnvironment(Symbol name, ref int frameIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var var = curframe.LookupLocal(name);
                if (var != null)
                {
                    frameIdx = FrameNum - curframe.FrameNum;
                    return curframe;
                }
                curframe = curframe.Parent;
            }

            return null;
        }

        internal AstBinding LookupAstRecursively(Symbol name, ref int frameIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var variable = curframe.LookupLocal(name);
                if (variable != null)
                {
                    frameIdx = FrameNum - curframe.FrameNum;
                    if (variable is AstBinding)
                        return variable as AstBinding;
                    else
                        return null;
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
        internal AstBinding LookupAst(Symbol name)
        {
            var variable = LookupLocal(name);
            if (variable == null)
                return null; 
            if (variable is AstBinding)
                return variable as AstBinding;
            else
                throw new Exception();
        }

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal AstBinding LockupAstRecursively(Symbol name)
        {
            int frameIdx = 0;
            return LookupAstRecursively(name, ref frameIdx);
        }

        /// <summary>
        /// Define new definition.
        /// </summary>
        /// <param name="name">identifier</param>
        /// <param name="binding">binding</param>
        public int Define(Symbol name, AstBinding binding)
        {
            var variable = LookupLocal(name);
            if (variable != null)
                throw SchemeError.SyntaxError("define", "environment already have key", binding.Id);

            binding.environment = this;
            binding.VarIdx = Bindings.Count;
            Bindings[name] = binding;
            return binding.VarIdx;
        }

        /// <summary>
        /// Define new definition.
        /// </summary>
        /// <param name="binding">binding</param>
        public int Define(AstBinding binding)
        {
            return Define(binding.Identifier, binding);
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
            var symbol = Symbol.Intern(name);
            var syntax = new Syntax(symbol);
            var binding = new PrimitiveBinding(syntax, primitive);
            binding.VarIdx = Bindings.Count;
            Define(symbol, binding);
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
            while (env != null)
            {
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
            while (index > 0 && env.Parent != null)
                env = env.Parent;
            return env;
        }


        #endregion
    }
}