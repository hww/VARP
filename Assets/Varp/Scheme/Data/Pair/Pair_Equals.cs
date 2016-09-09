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
        public static bool Equals(Pair from, object obj)
        {
            // TODO: a danger: what if this.Car introduces a loop? This is an infinite loop at the moment.
            if (obj == null) return false;
            if (!(obj is Pair)) return false;

            Pair current1 = from;
            Pair current2 = (Pair)obj;

            Pair loopHead1 = GetLoopHead(current1);
            Pair loopHead2 = GetLoopHead(current2);

            bool visitedLoopHead = false;

            // If one object contains a loop, so must the other
            if ((loopHead1 == null || loopHead2 == null) && loopHead1 != loopHead2)
                return false;

            while (current1 != null)
            {
                if (current1 == loopHead1)
                {
                    // The pair we're comparing to must also loop here
                    if (current2 != loopHead2)
                        return false;

                    // Stop if we've already been here
                    if (visitedLoopHead)
                        break;
                    else
                        visitedLoopHead = true;
                }

                // Cars must be equal (DANGER: CAR LOOPS ARE NOT ACCOUNTED FOR YET)
                if (current1.Car == null)
                {
                    if (current1.Car != null)
                        return false;
                }
                else if (!current1.Car.Equals(current2.Car))
                {
                    return false;
                }

                // Move on
                if (current1.Cdr == null)
                {
                    if (current2.Cdr == null)
                        return true;
                    else
                        return false;
                }
                else if (current1.Cdr is Pair)
                {
                    if (!(current2.Cdr is Pair))
                        return false;

                    current1 = (Pair)current1.Cdr;
                    current2 = (Pair)current2.Cdr;
                }
                else
                {
                    if (current2.Cdr is Pair)
                        return false;

                    return current1.Cdr.Equals(current2.Cdr);
                }
            }

            // Both pairs are equal
            return true;
        }
    }
}
