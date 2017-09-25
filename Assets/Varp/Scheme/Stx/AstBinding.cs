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
    using VM;

    public abstract class AstBinding : Binding
    {
        public Syntax Id;           //< the variable name definition
        public int VarIdx;          //< the variable indexs

        /// <summary>
        /// Create global binding
        /// </summary>
        /// <param name="variable"></param>
        protected AstBinding(Syntax variable)
        {
            Debug.Assert(variable != null);
            Id = variable;
        }

        /// <summary>
        /// variable identifier
        /// </summary>
        public virtual Symbol Identifier { get { return Id.AsIdentifier(); } }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<binding {0}>", Identifier.Name); }
        #endregion
    }

    public sealed class PrimitiveBinding : AstBinding
    {
        public delegate AST CompilerPrimitive(Syntax expression, Environment context);

        public CompilerPrimitive Primitive;     

        public PrimitiveBinding(Syntax identifier, CompilerPrimitive primitive) 
            : base (identifier)
        {
            Debug.Assert(primitive != null);
            Primitive = primitive;
        }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<primitive {0}>", Identifier.Name); }
        #endregion
    }
    
    public sealed class LocalBinding: AstBinding
    {
        public LocalBinding(Syntax identifier) : base(identifier) { }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<local-binding {0}>", Identifier.Name); }
        #endregion 
    }
    
    public sealed class GlobalBinding : AstBinding
    {
        public GlobalBinding(Syntax identifier) : base(identifier) { }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<global-binding {0}>", Identifier.Name); }
        #endregion 
    }
    
    public sealed class UpBinding : AstBinding
    {
        public int UpEnvIdx;
        public int UpVarIdx;

        public UpBinding(Syntax identifier, int envIdx, int varIdx) 
            : base(identifier)
        {
            UpEnvIdx = envIdx;
            UpVarIdx = varIdx;
        }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<up-binding {0}>", Identifier.Name); }
        #endregion

    }

    public sealed class ArgumentBinding : AstBinding
    {
        public enum Type
        {
            Required,       // (lambda (x y z) ...)
            Optionals,      // (lambda (x y #!optional z) ...)
            Key,            // (lambda (x y #!key z) ...)
            Rest,           // (lambda (x y #!rest z) ...)
            Body,           // (lambda (x y #!body z) ...)
            Define,
            End             // after #!res value
        }

        public Type ArgType;
        public AstBinding Refrence;
        public AST Initializer;

        public ArgumentBinding(Syntax identifier, Type type, AST initializer) : base(identifier)
        {
            ArgType = type;
            Initializer = initializer;
        }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<arg-binding {0}>", Identifier.Name); }
        #endregion
    }

}
