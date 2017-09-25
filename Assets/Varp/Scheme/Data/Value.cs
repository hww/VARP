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


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VARP.Scheme.Data
{
    /// <summary>
    /// Fields of the class
    /// </summary>
    public partial struct Value
    {
        internal double NumVal;
        internal object RefVal;

        #region Constructors
        public Value(ValueType type)
        {
            RefVal = type;
            NumVal = 0;
        }
        public Value(BoolType value)
        {
            RefVal = value;
            NumVal = 0;
        }
        public Value(char value)
        {
            RefVal = CharType.Empty;
            NumVal = value;
        }

        public Value(bool value)
        {
            RefVal = value ? BoolType.True : BoolType.False;
            NumVal = 0;
        }

        public Value(int value)
        {
            RefVal = NumericalType.Fixnum;
            NumVal = value;
        }

        public Value(uint value)
        {
            RefVal = NumericalType.Fixnum;
            NumVal = value;
        }

        public Value(double value)
        {
            RefVal = NumericalType.Float;
            NumVal = value;
        }

        public Value(object value) : this()
        {
            Set(value);
        }
        #endregion

    }


 
}
