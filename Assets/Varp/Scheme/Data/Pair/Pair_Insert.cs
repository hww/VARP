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
using System;

namespace VARP.Scheme.Data
{
    using Exception;
    using REPL;

    public sealed partial class Pair : SObject, ICollection<SObject>, IList<SObject>
    {
        /// <summary>
        /// Inserts an object as the Car of a new Pair at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <remarks>As with all IList operations, the effect may be different to what you expect if the index is 0</remarks>
        public static void Insert(Pair from, int index, SObject value)
        {
            // Can't remove before index 0
            if (index < 0) throw new IndexOutOfRangeException("Attempt to insert before a negative index from a Pair");

            if (index == 0)
            {
                // Create a new pair that's a copy of this one
                Pair newPair = new Pair(from.Car, from.Cdr);

                // Change this one to point to the new one and contain the object
                from.Cdr = newPair;
                from.Car = value;
            }
            else
            {
                // Get the preceeding Pair
                Pair preceeding = PairAtIndex(from, index - 1);

                // Create a new pair after preceeding
                preceeding.Cdr = new Pair(value, preceeding.Cdr);
            }
        }
    }
}
