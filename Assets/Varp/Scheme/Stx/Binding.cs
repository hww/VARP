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


    public sealed class Binding : SObject
    {
        public delegate AST CompilerPrimitive(Syntax expression, Environment context);

        public Symbol Identifier;               //< variable index in the environment
        public Symbol UID;                      //< unique id of variable name.number
        public int Index;                       //< unique id of variable name.number
        public CompilerPrimitive Primitive;     //< in case if primitive

        // define global variable
        public Binding(Symbol variable, CompilerPrimitive primitive = null)
        {
            Debug.Assert(variable != null);
            this.Identifier = variable;
            this.UID = null;
            this.Index = -1; // global!
            this.Primitive = primitive;
        }
        public Binding(Symbol variable, int index, CompilerPrimitive primitive = null)
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

}
