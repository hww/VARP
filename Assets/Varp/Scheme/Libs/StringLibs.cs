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

using System.Globalization;
using System.Diagnostics;

namespace VARP.Scheme.Libs
{
    using Data;
    using Exception;

    public static partial class StringLibs
    {
        public static int GetInteger(string value)
        {
            try
            {
                int val = int.Parse(value, NumberStyles.AllowLeadingSign);
                return val;
            }
            catch (System.Exception ex)
            {
                throw SchemeError.Error("get-integer", "improperly formed int value", value);
            }
        }

        public static int GetHexadecimal(string value)
        {
            try
            {
                Debug.Assert(value.Length > 2, "Error in hex literal");
                int hval = int.Parse(value.Substring(2), NumberStyles.AllowHexSpecifier);
                return hval;
            }
            catch (System.Exception ex)
            {
                throw SchemeError.Error("get-hexadecimal", "improperly formed int value", value);
            }
        }

        public static float GetFloat(string value)
        {
            float val = 0;

            if (float.TryParse(value, out val))
                return val;

            throw SchemeError.Error("get-float", "improperly formed float value", value);
        }

        public static double GetDouble(string value)
        {
            double val = 0;

            if (double.TryParse(value, out val))
                return val;

            throw SchemeError.Error("get-double", "improperly formed float value", value);
        }

        /// <summary>
        /// Return double value from any type of string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetNumerical(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw SchemeError.Error("get-double", "unexpected empty string", value);
            if (char.IsDigit(value[0]))
                return GetDouble(value);

            if (value[0] == '#')
                return GetHexadecimal(value);

            throw SchemeError.Error("get-numerical", "improperly formed numerical value", value);
        }
    }
}
