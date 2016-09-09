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
        /// Determines if this Pair represents a list containing a loop.
        /// It pass list by two iterator's one is slow another is fast
        /// fast makes 2 increments while slow does only one. If the 
        /// list is looped, iterators have to run forever. But at some moment 
        /// fast is reach same cell as slow is.
        /// </summary>
        /// <returns>true if this list is self-referential, false otherwise</returns>
        public static bool HasLoop(Pair from)
        {
            if (from.Cdr == null || !(from.Cdr is Pair)) return false;

            // Uses the tortoise & hare algorithm
            Pair slow = from;
            Pair fast = (Pair)from.Cdr;

            while (fast != null)
            {
                // We only check type for the fast value (as this will not change by the time the slow value gets there)
                if (slow == fast) return true;
                if (fast.Cdr == null) return false;
                if (!(fast.Cdr is Pair)) return false;

                fast = (Pair)fast.Cdr;

                if (slow == fast) return true;
                if (!(fast.Cdr is Pair)) return false;

                fast = (Pair)fast.Cdr;

                slow = (Pair)slow.Cdr;
            }

            return false;
        }
    }
}
