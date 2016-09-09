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
        /// <example>
        ///     ListFromArguments(1,2,3,4);
        /// </example>
        public static Pair ListFromArguments(params SObject[] objects)
        {
            return ListFromCollection(objects);
        }

        /// <example>
        ///     Pair last = null;
        ///     ListFromArguments(ref last, 1,2,3,4);
        /// </example>
        public static Pair ListFromArguments(ref Pair last, params SObject[] objects)
        {
            return ListFromCollection(objects, ref last);
        }

        public static Pair ListFromCollection(ICollection<SObject> collection, bool improper = false)
        {
            // null if there's nothing in the collection
            if (collection == null || collection.Count <= 0) return null;

            // Create a pair otherwise
            return new Pair(collection, improper);

        }
        public static Pair ListFromCollection(ICollection<SObject> collection, ref Pair last, bool improper = false)
        {
            // null if there's nothing in the collection
            if (collection == null || collection.Count <= 0) return null;

            // Create a pair otherwise
            return new Pair(collection, ref last, improper);
        }

        public static Pair ListFromList(IList<SObject> collection, int offset, bool improper = false)
        {
            // null if there's nothing in the collection
            if (collection == null || collection.Count <= 0) return null;
            // Create a pair otherwise
            return new Pair(collection, offset, improper);
        }
    }
}
