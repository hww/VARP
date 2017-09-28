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

namespace VARP.Scheme.VM.Functions
{
    using VARP.DataStructures;
    using VARP.Scheme.Data;
    using VARP.Scheme.Exception;
    using VARP.Scheme.Stx;

    public abstract class Function : SObject
    {

        /// <summary>
        /// Native functions will be called by VM
        /// wit three arguments: CALL A.B.C
        /// 
        /// Operand 'A' contains the index of result 
        /// Operand 'C' is quantity of results
        /// 0: NILL 0 no result
        /// 1: R(A) 
        /// 2: R(A..B)
        /// 3: R(A..C)
        /// Operand 'B' is quantity of arguments
        /// 0: () no arguments
        /// 1: R(A+1)
        /// 2: R(A+1..A+2)
        /// 3: R(A+1..A+3)
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>        
        public abstract void Call(Frame frame, int a, int b, int c);

        #region Assertions

        private static SchemeError ArityError(string name, string message, int argidx, int given, int expected, Value[] argv)
        {
            var arguments = new LinkedList<Value>();
            if (given > 0)
            {
                var lastidx = argidx + given;
                for (var i = argidx; i < lastidx; i++) arguments.AddLast(argv[i]);
            }
            return new SchemeError(SchemeError.ArityErrorMessage(name, message, expected, given, arguments, null));
        }

        protected static void AssertArgsMinimum(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given < expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }

        protected static void AssertArgsMaximum(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given > expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }

        protected static void AssertArgsEqual(string name, string message, int argidx, int given, int expected, Frame frame)
        {
            if (given != expected)
                throw ArityError(name, message, argidx, given, expected, frame.Values);
        }

        #endregion

        #region SObject

        public override bool AsBool() { return true; }
        public override string ToString() { return string.Format("#<function {0}>", this.GetType().Name); }
        public override string Inspect() { return ToString(); }

        #endregion
    }
}
