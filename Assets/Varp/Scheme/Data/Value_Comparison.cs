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

namespace VARP.Scheme.Data
{
    /// <summary>
    /// Value constructors
    /// </summary>
    public partial struct Value
    {

        #region GetHashCode

        internal static int GetHashCode(bool value)
        {
            return value ? 1 : 0;
        }

        internal static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        internal static int GetHashCode(object value)
        {
            return System.Runtime.CompilerServices.
                RuntimeHelpers.GetHashCode(value);
        }

        public override int GetHashCode()
        {
            if (RefVal == null || RefVal == BoolClass.False)
                return 0;

            if (RefVal == BoolClass.True)
                return 1;

            if (RefVal == FloatClass.Instance)
                return GetHashCode(NumVal);

            return GetHashCode(RefVal);
        }

        #endregion

        #region Equals

        public bool Equals(bool value)
        {
            return value ? RefVal is TrueClass : RefVal is FalseClass;
        }

        public bool Equals(int value)
        {
            return RefVal is NumberClass && (int)NumVal == value;
        }

        public bool Equals(double value)
        {
            return RefVal is NumberClass && NumVal == value;
        }

        public bool Equals(string str)
        {
            return RefVal is string &&  (RefVal as string).Equals(str);
        }

        public bool Equals(Value other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            var asVal = obj is Value ? (Value)obj : new Value(obj);
            return Equals(this, asVal);
        }

        public static bool Equals(Value a, Value b)
        {
            if (a.RefVal is NumberClass && b.RefVal is NumberClass)
                return a.NumVal == b.NumVal;

            if (a.RefVal == b.RefVal)
                return true;

            if (a.RefVal is ValueClass)
                return (a.RefVal as ValueClass).Equals(b);

            return false;
        }

        #endregion

        #region Operators

        public static bool operator ==(Value a, Value b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Value a, Value b)
        {
            return !Equals(a, b);
        }

        public static bool operator ==(Value v, bool b)
        {
            return v.Equals(b);
        }

        public static bool operator !=(Value v, bool b)
        {
            return !v.Equals(b);
        }

        public static bool operator ==(Value v, int n)
        {
            return v.Equals(n);
        }

        public static bool operator ==(Value v, uint n)
        {
            return v.Equals(n);
        }

        public static bool operator ==(Value v, double n)
        {
            return v.Equals(n);
        }

        public static bool operator !=(Value v, int n)
        {
            return !v.Equals(n);
        }

        public static bool operator !=(Value v, uint n)
        {
            return !v.Equals(n);
        }

        public static bool operator !=(Value v, double n)
        {
            return !v.Equals(n);
        }

        public static bool operator ==(Value v, string s)
        {
            return v.Equals(s);
        }

        public static bool operator !=(Value v, string s)
        {
            return !v.Equals(s);
        }

        public static bool operator ==(Value v, object o)
        {
            return v.Equals(o);
        }

        public static bool operator !=(Value v, object o)
        {
            return !v.Equals(o);
        }

        #endregion

    }
}