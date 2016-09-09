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

namespace VARP.Scheme.Stx.Primitives
{
    using Exception;
    using Data;

    public class BasePrimitive
    {
        #region Assertions
        protected static int GetArgsCount(SObject o) { return Pair.Length(o) - 1; }
        protected static void AssertArgsMinimum(Syntax syntax, int minimum, int given) { if (given < minimum) throw new ArityMissmach(syntax, minimum, given, ArityMissmach.Options.Minimum); }
        protected static void AssertArgsMaximum(Syntax syntax, int maximum, int given) { if (given > maximum) throw new ArityMissmach(syntax, maximum, given, ArityMissmach.Options.Minimum); }
        protected static void AssertArgsEqual(Syntax syntax, int required, int given) { if (given != required) throw new ArityMissmach(syntax, required, given, ArityMissmach.Options.Equal); }
        protected static void AssertArgsMinimum(Syntax syntax, int minimum, int given, string message) { if (given < minimum) throw new ArityMissmach(syntax, minimum, given, message, ArityMissmach.Options.Minimum); }
        protected static void AssertArgsMaximum(Syntax syntax, int maximum, int given, string message) { if (given > maximum) throw new ArityMissmach(syntax, maximum, given, message, ArityMissmach.Options.Minimum); }
        protected static void AssertArgsEqual(Syntax syntax, int required, int given, string message) { if (given != required) throw new ArityMissmach(syntax, required, given, message, ArityMissmach.Options.Equal); }
        #endregion
    }
}