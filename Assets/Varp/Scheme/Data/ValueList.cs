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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VARP.Scheme.Data
{
    using Data;
    using DataStructures;

    public static class ValueList
    {
        public static List<Value> FromArguments(params Value[] args)
        {
            return new List<Value>(args);
        }
        public static List<Value> FromArguments(params object[] args)
        {
            List<Value> list = new List<Value>(args.Length);
            foreach (var o in args)
                list.Add(new Value(o));
            return list;
        }
        public static List<Value> FromList<T>(List<T> args)
        {
            List<Value> list = new List<Value>(args.Count);
            foreach (var o in args)
                list.Add(new Value(o));
            return list;
        }

        public static string ToString<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            bool appendSpace = false;
            sb.Append("#(");
            foreach (var v in list)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(ValueString.ToString(v));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }

    }
}
