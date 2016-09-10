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

using System;

namespace VARP.Scheme.REPL
{
    /// <summary>
    /// Format string, and apply inspector before object with format starts with '?'
    /// </summary>
    public class SchemeFormatter : IFormatProvider, ICustomFormatter
    {
        // String.Format calls this method to get an instance of an 
        // ICustomFormatter to handle the formatting. 
        public object GetFormat(Type service)
        {
            if (service == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }
        // After String.Format gets the ICustomFormatter, it calls this format 
        // method on each argument. 
        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (format == null)
            {
                return String.Format("{0}", arg);
            }
            // If the format is not a defined custom code, 
            // use the formatting support in ToString. 
            if (!format.StartsWith("?"))
            {
                //If the object to be formatted supports the IFormattable 
                //interface, pass the format specifier to the  
                //object's ToString method for formatting. 
                if (arg is IFormattable)
                {
                    return ((IFormattable)arg).ToString(format, provider);
                }
                //If the object does not support IFormattable,  
                //call the object's ToString method with no additional 
                //formatting.  
                else if (arg != null)
                {
                    return arg.ToString();
                }
            }
            // form the output string.
            string format1 = format.Remove(0, 1); // remove '?'
            return string.Format(format1, Inspector.Inspect(arg));
        }
    }
}
