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
        /// If this pair contains a loop, finds the first element that exists in the loop
        /// </summary>
        /// <returns>The element that begins the loop, or null if there is not a loop</returns>
        /// <remarks>It is more efficient to call this and check for a null return than it is to call HasLoop() beforehand if you need
        /// to determine if something contains a loop, and if it does, where that loop begins.</remarks>
        public static Pair GetLoopHead(Pair from)
        {
            // Found an O(n) algorithm for this at http://discuss.fogcreek.com/techInterview/default.asp?cmd=show&ixPost=710
            // (This is Alex Harris's algorithm - I think I agree with his reasoning that this is a O(n) operation)

            Pair x = new Pair(null, from);                  // An element outside the loop
            Pair y = from;                                  // (Not necessarily an element inside the loop)

            // Change y to an element inside the loop
            Pair slow = x;
            Pair fast = from;

            for (;;)
            {
                // We only check type for the fast value (as this will not change by the time the slow value gets there)
                if (slow == fast) { y = slow; break; }
                if (!(fast.Cdr is Pair)) return null;
                if (fast.Cdr == null) return null;

                fast = (Pair)fast.Cdr;

                if (slow == fast) { y = slow; break; }
                if (!(fast.Cdr is Pair))
                    return null;
                if (fast.Cdr == null) return null;

                fast = (Pair)fast.Cdr;

                slow = (Pair)slow.Cdr;
            }

            // Find the loop head
            Pair a = x;
            Pair b = (Pair)y.Cdr;
            Pair c = y;

            for (;;)
            {
                Pair t = Midpoint(a, c);
                if (Find(b, t, c))
                    c = t;
                else
                    a = (Pair)t.Cdr;

                t = Midpoint(b, c);

                if (Find(a, t, c))
                    c = t;
                else
                    b = (Pair)t.Cdr;

                if (a == b) return a;
                if (a == c || b == c) return c;
            }
        }
    }
}
