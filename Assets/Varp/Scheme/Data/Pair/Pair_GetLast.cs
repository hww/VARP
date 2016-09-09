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
        /// <summary>
        /// Return last element of list. It can be element with Cdr == null
        /// or Cdr is not Pair. In case of looped list this function will 
        /// produce exception
        /// </summary>
        /// <returns></returns>
        public static Pair GetLast(Pair from)
        {
            if (from.Cdr == null || !(from.Cdr is Pair)) return from;

            // Uses the tortoise & hare algorithm
            Pair slow = from;
            Pair fast = (Pair)from.Cdr;

            while (fast != null)
            {
                // We only check type for the fast value (as this will not change by the time the slow value gets there)
                if (slow == fast) break; //< looped
                if (fast.Cdr == null) return fast; //< last
                if (!(fast.Cdr is Pair)) return fast; //< last dot syntax

                fast = (Pair)fast.Cdr;

                if (slow == fast) break;
                if (fast.Cdr == null) return fast; //< last
                if (!(fast.Cdr is Pair)) return fast; //< last dot syntax

                fast = (Pair)fast.Cdr;

                slow = (Pair)slow.Cdr;
            }
            throw new SchemeException("Requested last element for looped list");
        }
    }
}
