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

using VARP.Scheme.Tokenizing;
using VARP.Scheme.Stx;

namespace VARP.Scheme.Exception
{
    /// <summary>
    /// General exception class
    /// </summary>
    public class SchemeException : System.ApplicationException
    {
        public SchemeException() : base()
        {
        }

        public SchemeException(string message) : base(message)
        {
        }

        public SchemeException(string message, System.Exception innerException) : base(message, innerException)
        {
        }


        static protected string GetLocationString(object x)
        {
            if (x is Location)
                return GetLocationStringIntern(x as Location);
            if (x is Token)
                return GetLocationStringIntern((x as Token).location);
            if (x is Syntax)
                return GetLocationStringIntern((x as Syntax).location);
            return string.Empty;
        }
        static string GetLocationStringIntern(Location x)
        {
            return x==null ? string.Empty : string.Format("{0}({1},{2})", x.File, x.LineNumber, x.ColNumber);
        }
    }

}
