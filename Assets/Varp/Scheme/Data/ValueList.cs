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
using System.Diagnostics;

namespace VARP.Scheme.Data
{
    using DataStructures;
    using REPL;
    using System.Text;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class ValueList : LinkedList<Value>
    {
        public ValueList() : base()
        {
        }

        public ValueList(LinkedList<Value> list) : base()
        {
            head = list.head;
            count = list.count;
        }

        public static ValueList ListFromArguments(params Value[] args)
        {
            return new ValueList(args);
        }
        public static ValueList ListFromArguments(params object[] args)
        {
            return new ValueList(args);
        }

        public ValueList(IEnumerable<Value> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
                AddLast(item);
        }

        public ValueList(IEnumerable<object> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
                AddLast(new Value(item));
        }

        public Value ToValue()
        {
            return new Value(this);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");

            LinkedListNode<Value> curent = First;
            while (curent != null)
            {
                Symbol sym = curent.Value.AsSymbol();
                if (sym!=null && sym.IsSpecialForm)
                    sb.Append(sym.ToSpecialFormString());
                else
                    sb.Append(ValueString.ToString(curent.Value));

                curent = curent.Next;
                if (curent != null) sb.Append(" ");
            }
            sb.Append(")");
            return sb.ToString();
        }

        #region DebuggerDisplay 
        public string DebuggerDisplay
        {
            get
            {
                return string.Format("#<ValueList Count={0}>", Count);
            }
        }
        #endregion
    }
}
