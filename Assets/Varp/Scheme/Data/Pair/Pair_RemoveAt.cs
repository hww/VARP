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
        /// Removes the Pair at the specified index.
        /// </summary>
        /// <param name="index">The index of the object to remove.</param>
        /// <remarks>
        /// Removing the object at index 0 may have 'odd' side-effects (this effectively copies the object at index 1 over this one).
        /// You cannot remove the object at index 0 if this is the only object in the list. Use the Cdr for preference instead of trying
        /// to remove the first object if at all possible.
        /// </remarks>
        public static void RemoveAt(Pair from, int index)
        {
            // Can't remove before index 0
            if (index < 0) throw new IndexOutOfRangeException("Attempt to remove a negative index from a Pair");

            if (index == 0)
            {
                // Check for error conditions
                if (from.Cdr == null) throw new IndexOutOfRangeException("Cannot remove index 0 of a Pair if its Cdr is null");
                if (!(from.Cdr is Pair)) throw new IndexOutOfRangeException("Cannot remove index 0 of a pair if its Cdr is not also a Pair");

                // Replace ourself with the following object
                Pair following = (Pair)from.Cdr;

                from.Car = following.Car;
                from.Cdr = following.Cdr;
            }
            else
            {
                // At this point, index is > 0, so we remove by altering the Cdr of this element:
                Pair preceeding = PairAtIndex(from, index - 1);

                // Check that there's a following element, and that it's also a Pair
                if (preceeding.Cdr == null) throw new IndexOutOfRangeException("Attempt to remove an element beyond the end of a list");
                if (!(preceeding.Cdr is Pair)) throw new IndexOutOfRangeException("Cannot remove the final element of an improper list");

                // Remove the element
                preceeding.Cdr = ((Pair)preceeding.Cdr).Cdr;
            }
        }
    }
}
