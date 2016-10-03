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

    public class AstBinding : ValueClass
    {
        public Syntax Id;
        public byte EnvIdx;           //< the variable binded to environment
        public byte VarIdx;           //< variable index in the environment

        /// <summary>
        /// Create global binding
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="variable"></param>
        /// <param name="index"></param>
        public AstBinding(AstEnvironment environment, Syntax variable)
        {
            Debug.Assert(environment != null);
            Debug.Assert(variable != null);
            if (environment.Index > 255) SchemeError.Error("binding", "too many environments", variable);
            this.EnvIdx = (byte)environment.Index;
            this.Id = variable;
            this.VarIdx = 0;
        }


        /// <summary>
        /// variable identifier
        /// </summary>
        public virtual Symbol Identifier { get { return Id.AsIdentifier(); } }

        public virtual bool IsPrimitive { get { return false; } }
        public virtual bool IsGlobal { get { return EnvIdx < 0; } }
        public virtual bool IsLocal { get { return EnvIdx >= 0; } }
        public virtual bool IsUpvalue { get { return false; } }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<binding {0}>", Identifier.Name); }
        #endregion
    }
    public sealed class PrimitiveBinding : AstBinding
    {
        public delegate AST CompilerPrimitive(Syntax expression, AstEnvironment context);

        public CompilerPrimitive Primitive;     

        // define global variable
        public PrimitiveBinding(AstEnvironment environment, Syntax identifier, CompilerPrimitive primitive) : base (environment, identifier)
        {
            Debug.Assert(primitive != null);
            this.Primitive = primitive;
        }

        public override bool IsPrimitive { get { return true; } }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<primitive {0}>", Identifier.Name); }
        #endregion
    }
    public sealed class UpBinding : AstBinding
    {
        public byte RefEnvIdx;
        public byte RefVarIdx;
        public UpBinding(AstEnvironment environment, Syntax identifier, AstBinding other) : base(environment, identifier)
        {
            Debug.Assert(other != null);

            if (other is UpBinding)
            {
                UpBinding ub = other as UpBinding;
                this.RefEnvIdx = ub.RefEnvIdx;
                this.RefVarIdx = ub.RefVarIdx;
            }
            else
            {
                this.RefEnvIdx = other.EnvIdx;
                this.RefVarIdx = other.VarIdx;
            }
        }

        public byte EnvOffset { get { return (byte)(EnvIdx - RefEnvIdx); } }
        public override bool IsUpvalue { get { return true; } }

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
        public ArgumentBinding(AstEnvironment environment, Syntax identifier, Type type, AST initializer) : base(environment, identifier)
        {
            Debug.Assert(initializer != null);
            this.Initializer = initializer;
            this.ArgType = type;
        }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<opt-binding {0}>", Identifier.Name); }
        #endregion
    }

}
