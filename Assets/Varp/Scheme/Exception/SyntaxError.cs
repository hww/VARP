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

using VARP.Scheme.Stx;
using VARP.Scheme.Data;

namespace VARP.Scheme.Exception
{
    /// <summary>
    /// Exception thrown by the parser when a syntax error is found.
    /// </summary>
    public class SyntaxError : SchemeException
    {
        // ==========================================================================
        // Simple variant
        // ==========================================================================
        public SyntaxError() : base("Syntax error")
        {
        }
        public SyntaxError(string message) : base("Syntax error: " + message)
        {
        }
        public SyntaxError(string message, System.Exception innerException) : base("Syntax error: " + message, innerException)
        {
        }
        // ==========================================================================
        // With additional argument 
        // ==========================================================================
        public SyntaxError(object obj) : base("Syntax error: " + obj.ToString())
        {
        }
        public SyntaxError(string message, object obj) : base(string.Format("Syntax error: {0} {1}", obj.ToString(), message))
        {
        }
        public SyntaxError(string message, object obj, System.Exception innerException) : base(string.Format("Syntax error: {0} {1}", obj.ToString(), message), innerException)
        {
        }
        // ==========================================================================
        // With Location
        // ==========================================================================
        public SyntaxError(SObject obj) : base(GetLocationString(obj) + " Syntax error")
        {
        }
        public SyntaxError(SObject obj, string message) : base(GetLocationString(obj) + " Syntax error: " + message)
        {
        }
        public SyntaxError(SObject obj, string message, System.Exception innerException) : base(GetLocationString(obj) + " Syntax error: " + message, innerException)
        {
        }
    }
    public class ExpectedIdentifier : SchemeException
    {
        public ExpectedIdentifier(Syntax obj) : base(GetLocationString(obj) + " Expected identifier")
        {
        }
        public ExpectedIdentifier(Syntax obj, string message) : base(GetLocationString(obj) + " Expected identifier: " + message)
        {
        }
        public ExpectedIdentifier(Syntax obj, string message, System.Exception innerException) : base(GetLocationString(obj) + " Expected identifier: " + message, innerException)
        {
        }
    }
 
 
}

