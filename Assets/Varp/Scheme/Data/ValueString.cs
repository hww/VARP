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
using System.Globalization;

namespace VARP.Scheme.Data
{
    using DataStructures;
    using Exception;

    /// <summary>
    /// This class will have only static members
    /// </summary>
    public static class ValueString
    {
        /// <summary>
        /// TODO found better name
        /// This function have to put into double quotes the string
        /// value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddQuotes(string str)
        {
            return string.Format("\"{0}\"", str);
        }

        /// <summary>
        /// Convert to string any object and add quotes to strings
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToString(object val)
        {
            Debug.Assert(val != null);
            if (val is string)
                return ToString(val as string);
            if (val is Value)
                return ToString((Value)val);
            if (val is Symbol)
                return ToString((Symbol)val);
            if (val is List<Value>)
                return ToString(val as List<Value>);
            if (val is LinkedList<Value>)
                return ToString(val as LinkedList<Value>);
            if (val is Dictionary<object, Value>)
                return ToString(val as Dictionary<object, Value>);
            return val.ToString();
        }

        public static string ToString(Symbol sym)
        {
            Debug.Assert(sym != null);
            if (sym != null && sym.IsSpecialForm)
                return sym.ToSpecialFormString();
            else
                return sym.ToString();
        }

        public static string ToString(string val)
        {
            Debug.Assert(val != null);
            return AddQuotes(val.ToString());
        }
        public static string ToString(Value val)
        {
            if (val.IsString)
                return AddQuotes(val.ToString());
            return val.ToString();
        }
        public static string ToString(ValueClass val)
        {
            Debug.Assert(val != null);
            if (val is NumericalClass)
                throw SchemeError.Error("to-string", "can't inspect number-class", val);
            return val.ToString();
        }
        public static string ToString(List<Value> list)
        {
            Debug.Assert(list != null);
            return ValueList.ToString(list);
        }
        public static string ToString(LinkedList<Value> list)
        {
            Debug.Assert(list != null);
            return ValueLinkedList.ToString(list);
        }
        public static string ToString(Dictionary<object, Value> list)
        {
            Debug.Assert(list != null);
            return ValueDictionary.ToString(list);
        }
    }

}
