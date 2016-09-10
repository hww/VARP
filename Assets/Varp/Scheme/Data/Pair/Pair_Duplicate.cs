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
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <returns>new list</returns>
        /// <example>
        ///     SObject newList = Duplicate(sourceList);
        /// </example>
        public static Pair Duplicate(SObject sourceFirst)
        {
            Pair dstFirst = null;
            Pair dstLast = null;
            Duplicate(sourceFirst, null, ref dstFirst, ref dstLast);
            return dstFirst;
        }
        /// <summary>
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <param name="destination">destination</param>
        /// <returns>last element in new list</returns>
        /// <example>
        ///     Sobject newList = null;
        ///     int newSize = Duplicate(sourceList, ref newList);
        /// </example>
        public static int Duplicate(SObject sourceFirst, ref SObject dstFirst)
        {
            Pair first = null;
            Pair last = null;
            int size = Duplicate(sourceFirst, null, ref first, ref last);
            dstFirst = first as SObject;
            return size;
        }
        /// <summary>
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <param name="dstFirst">destination</param>
        /// <param name="dstLast">last element of destination list</param>
        /// <returns>quantity of elements</returns>
        /// <example>
        ///     Sobject newList = null;
        ///     Sobject newListLast = null;
        ///     int newSize = Duplicate(sourceList, ref newList, ref newListLast);
        /// </example>
        public static int Duplicate(SObject sourceFirst, ref SObject dstFirst, ref SObject dstLast)
        {
            Pair first = null;
            Pair last = null;
            int size = Duplicate(sourceFirst, null, ref first, ref last);
            dstFirst = first as SObject;
            dstLast = last as SObject;
            return size;
        }

        /// <summary>
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <param name="dstFirst">destination</param>
        /// <param name="dstLast">last element of destination list</param>
        /// <returns>quantity of elements</returns>
        /// <example>
        ///     Pair appendTo = null;
        ///     Pair newListLast = null;
        ///     int newSize = Duplicate(sourceList, ref appendTo.Cdr, ref newListLast);
        /// </example>
        public static int Duplicate(SObject sourceFirst, ref SObject dstFirst, ref Pair dstLast)
        {
            Pair first = null;
            Pair last = null;
            int size = Duplicate(sourceFirst, null, ref first, ref last);
            dstFirst = first as SObject;
            dstLast = last;
            return size;
        }

        /// <summary>
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <param name="dstFirst">destination</param>
        /// <param name="dstLast">last element of destination list</param>
        /// <returns>quantity of elements</returns>
        /// <example>
        ///     Pair newList = null;
        ///     Pair newListLast = null;
        ///     int newSize = Duplicate(sourceList, ref newList, ref newListLast);
        /// </example>
        public static int Duplicate(SObject sourceFirst, ref Pair dstFirst, ref Pair dstLast)
        {
            return Duplicate(sourceFirst, null, ref dstFirst, ref dstLast);
        }

        /// <summary>
        /// Duplicate list
        /// </summary>
        /// <param name="sourceFirst">source list</param>
        /// <param name="sourceLast">source last element. can be null</param>
        /// <param name="dstFirst">destination</param>
        /// <param name="dstLast">last element of destination list</param>
        /// <param name="allowImproper">allow make improper list</param>
        /// <returns>quantity of elements</returns>
        /// <example>
        ///     Pair elementAtIndex2 = sourceList.PairAtIndex(2);
        ///     Pair elementAtIndex5 = sourceList.PairAtIndex(5);
        ///     Pair newList = null;
        ///     Pair newListLast = null;
        ///     int newSize = Duplicate(elementAtIndex2, elementAtIndex5, ref newList, ref newListLast);
        /// </example>
        public static int Duplicate(SObject sourceFirst, Pair sourceLast, ref Pair dstFirst, ref Pair dstLast)
        {
            if (sourceFirst == null)
            {
                dstFirst = dstLast = null;
                return 0;
            }
            if (!(sourceFirst is Pair)) throw new ContractViolation("list?", Inspector.Inspect(sourceFirst), "duplicate:");
            Pair fast = sourceFirst as Pair;
            Pair loopHead = GetLoopHead(fast); // element which point inside the list
            PairSpan span = new PairSpan(new Pair());

            int count = 1;

            while (fast != null)
            {
                span.Last.Car = fast.Car; 

                if (fast.Cdr == null) break;
                if (fast.Cdr is Pair)
                {
                    if (fast == sourceLast) break; // found last element. only for proper!
                    count++;
                    fast = fast.Cdr as Pair;
                    span.AddPairLast(new Pair());
                }
                else
                {
                    span.Last.Cdr = fast.Cdr;
                    break;
                }

                if (fast == null) break;
                if (fast == loopHead) break; // found loop

                span.Last.Car = fast.Car; 
                if (fast.Cdr == null) break;
                if (fast.Cdr is Pair)
                {
                    if (fast == sourceLast) break; // found last element. only for proper!
                    count++;
                    fast = fast.Cdr as Pair;
                    span.AddPairLast(new Pair());
                }
                else
                {
                    span.Last.Cdr = fast.Cdr;
                    break;
                }
            }
            dstFirst = span.First; //< return result
            dstLast = span.Last; //< return last element of list
            return count;
        }
    }
}
