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

using VARP.Scheme.Data;

namespace VARP.Scheme.Exception
{
    public class ContractViolation : SchemeException
    {
        public ContractViolation(SObject obj, string expected, string given) : base(Format(obj, expected, given))
        {
        }
        public ContractViolation(SObject obj, string expected, string given, string message) : base(Format(obj, expected, given, message))
        {
        }
        public ContractViolation(SObject obj, string expected, string given, string message, System.Exception innerException) : base(Format(obj, expected, given, message), innerException)
        {
        }
        static string Format(SObject obj, string expected, string given)
        { 
            return string.Format("{0} contract violation: expected: {1} given: {2}", GetLocationString(obj), expected, given);
        }
        static string Format(SObject obj, string expected, string given, string message)
        {
            return string.Format("{0} {1} contract violation: expected: {2} given: {3}", GetLocationString(obj), message, expected, given);
        }


        public ContractViolation(string expected, string given) : base(Format(expected, given))
        {
        }
        public ContractViolation(string expected, string given, string message) : base(Format(expected, given, message))
        {
        }
        public ContractViolation(string expected, string given, string message, System.Exception innerException) : base(Format(expected, given, message), innerException)
        {
        }
        static string Format(string expected, string given)
        {
            return string.Format("contract violation: expected: {0} given: {1}", expected, given);
        }
        static string Format(string expected, string given, string message)
        {
            return string.Format("{0} contract violation: expected: {1} given: {2}", message, expected, given);
        }



    }

}
