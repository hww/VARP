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

namespace VARP.Scheme.Data
{
    /// This value can be used when C# function have to return scheme object
    /// But there is nothing to return and there can't be used NULL as returns
    /// too
    ///
    ///     Example if C# implementation of the function 'display'. In the Lisp
    ///     expression looks like (display 1)
    ///
    ///     SObject display(...)
    ///     {
    ///        // Do display value
    ///        return SVoid.VoidValue
    ///     }
    ///
    /// Difference between SVoid and SNull: SVoid can be recognized as empty list
    /// As it can be with SNull
    public sealed class SVoid : SObject
    {
        public SVoid() { }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override bool Equals(object obj)
        {
            return obj == this;
        }

        #region SObject Methods
        public override bool IsLiteral { get { return true; } }

        public SBool ToBool() { return SBool.False; }
        public override string AsString() { return string.Format("void"); }

        #endregion
    }
}
