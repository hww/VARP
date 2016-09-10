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

namespace VARP.Scheme.Data
{
    using Exception;
    using REPL;

    public sealed partial class Pair : SObject, ICollection<SObject>, IList<SObject>
    {

        // TODO! make it self checking looped or improper
        public static Pair Reverse(SObject obj)
        {
            if (obj == null) return null;
            if (!(obj is Pair))
                throw SchemeError.ArgumentError("reverse", "list?", obj);

            Pair fast = obj as Pair;
            Pair slow = fast;
            Pair reversed = null;

            while (fast != null)
            {
                reversed = new Pair(fast.Car, reversed);
                if (fast.Cdr == null) break;
                if (fast.Cdr is Pair)
                    fast = fast.Cdr as Pair;
                else
                    throw new SchemeError("Reverse: Can't reverse looped or improper list");

                if (fast == null) break;
                if (fast == slow) throw new SchemeError("Reverse: Can't reverse looped or improper list");

                reversed = new Pair(fast.Car, reversed);
                if (fast.Cdr == null) break;
                if (fast.Cdr is Pair)
                    fast = fast.Cdr as Pair;
                else
                    throw new SchemeError("Reverse: Can't reverse looped or improper list");
                slow = slow.Cdr as Pair;
            }
            return reversed;
        }
    }
}
