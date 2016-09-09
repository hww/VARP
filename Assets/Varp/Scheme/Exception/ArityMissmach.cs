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
    public class ArityMissmach : SchemeException
    {
        public enum Options
        {
            Equal,
            Minimum,
            Maximum
        }
        public ArityMissmach(SObject location, int expected, int given, Options options = Options.Equal) : base(Format(location, expected, given, options))
        {
        }
        public ArityMissmach(SObject location, int expected, int given, string message, Options options = Options.Equal) : base(Format(location, expected, given, message, options))
        {
        }
        public ArityMissmach(SObject location, int expected, int given, string message, System.Exception innerException, Options options = Options.Equal) : base(Format(location, expected, given, message, options), innerException)
        {
        }


        static string Format(SObject location, int expected, int given, Options options = Options.Equal)
        {
            switch (options)
            {

                case Options.Minimum:
                    return string.Format("arity mismatch; expected more or equal: {0} given: {1} arguments...:", expected, given);

                case Options.Maximum:
                    return string.Format("arity mismatch;  expected minimum: {0} given: {1}  arguments...:",  expected, given);

                case Options.Equal:
                    break;
            }
            return string.Format("arity mismatch; expected: {0} given: {1}  arguments...:", expected, given);
        }
        static string Format(SObject location, int expected, int given, string message, Options options = Options.Equal)
        {
            switch (options)
            {

                case Options.Minimum:
                    return string.Format("{2} arity mismatch; expected more or equal: {0} given: {1} arguments...:", expected, given, message);

                case Options.Maximum:
                    return string.Format("{2} arity mismatch;  expected minimum: {0} given: {1} arguments...:", expected, given, message);

                case Options.Equal:
                    break;
            }
            return string.Format("{2} arity mismatch; expected: {0} given: {1} arguments...:", expected, given, message);
        }
    }
}
