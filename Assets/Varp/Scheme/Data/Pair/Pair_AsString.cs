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
        public static string AsString(Pair first)
        {
            string res = "";

            Pair current = first;

            Pair loopHead = GetLoopHead(first);        // The first item in the loop if this pair contains one
            bool visitedLoopHead = false;               // set to true after the first time we visit loopHead

            while (true)
            {
                if (current.Car == null)
                    res += "()";
                else
                    res += current.Car.AsString();

                if (current.Cdr == null)
                {
                    // We're done
                    break;
                }
                else if (current.Cdr is Pair)
                {
                    // The Cdr is a pair: move on
                    res += " ";
                    current = (Pair)current.Cdr;

                    if (current == loopHead && visitedLoopHead)
                    {
                        // Notify that this contains a loop
                        res += "#[...]";
                    }

                    if (current == loopHead) visitedLoopHead = true;
                }
                else
                {
                    // The Cdr is not a pair add a '. foo' style thing and finish
                    res += " . " + current.Cdr.AsString();
                    current = null;
                    break;
                }
            }

            // We've constructed everything except the brackets
            return "(" + res + ")";
        }
    }
}
