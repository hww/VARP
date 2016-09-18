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
using System.Text;

namespace VARP.Scheme.Data
{
    public sealed class ValueDictionary
    {
        public static Dictionary<object, Value> FromValuePairArray(ValuePair[] array)
        {
            Dictionary<object, Value> list = new Dictionary<object, Value>();
            foreach (var v in array)
                list[v.Item1] = v.Item2;
            return list;
        }

        public static Dictionary<object, Value> FromArguments(params Value[] args)
        {
            Debug.Assert((args.Length & 1) == 0);
            ValuePair[] list = new ValuePair[args.Length / 2];
            for (int i = 0; i < args.Length; i += 2)
                list[i / 2] = new ValuePair(args[i], args[i + 1]);
            return FromValuePairArray(list);
        }
        public static Dictionary<object, Value> FromArguments(params object[] args)
        {
            Debug.Assert((args.Length & 1) == 0);
            ValuePair[] list = new ValuePair[args.Length / 2];
            for (int i = 0; i < args.Length; i += 2)
                list[i / 2] = new ValuePair(args[i], args[i + 1]);
            return FromValuePairArray(list);
        }

        public static string ToString(Dictionary<object, Value> table)
        {
            StringBuilder sb = new StringBuilder();
            bool appendSpace = false;
            sb.Append("#hash(");
            foreach (var v in table)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(string.Format("({0} . {1})", ValueString.ToString(v.Key), ValueString.ToString(v.Value)));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
