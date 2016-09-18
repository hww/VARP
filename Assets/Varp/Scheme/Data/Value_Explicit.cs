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


namespace VARP.Scheme.Data
{
    /// <summary>
    /// Value constructors
    /// </summary>
    public partial struct Value
    {

        #region explicit operator

        //these are similar to the To* methods
        //except they throw InvalidCastException
        //if the underlying type doesn't match

        public static explicit operator bool(Value value)
        {
            if (value.RefVal is TrueClass)
                return true;
            if (value.RefVal is FalseClass)
                return false;

            throw new InvalidCastException();
        }

        public static explicit operator char(Value value)
        {
            if (value.RefVal is CharClass)
                return (char)value.NumVal;
            throw new InvalidCastException();
        }

        public static explicit operator int(Value value)
        {
            if (value.RefVal is NumberClass)
                return (int)value.NumVal;
            throw new InvalidCastException();
        }

        public static explicit operator uint(Value value)
        {
            if (value.RefVal is NumberClass)
                return (uint)value.NumVal;
            throw new InvalidCastException();


        }

        public static explicit operator double(Value value)
        {
            if (value.RefVal is NumberClass)
                return value.NumVal;
            throw new InvalidCastException();
        }

        public static explicit operator float(Value value)
        {
            if (value.RefVal is NumberClass)
                return (float)value.NumVal;
            throw new InvalidCastException();
        }

        public static explicit operator string(Value value)
        {
            return (string)value.RefVal;
        }

        public static explicit operator ValueVector(Value value)
        {
            return (ValueVector)value.RefVal;
        }

        public static explicit operator ValuePair(Value value)
        {
            return (ValuePair)value.RefVal;
        }

        public static explicit operator ValueList(Value value)
        {
            return (ValueList)value.RefVal;
        }

        public static explicit operator Symbol(Value value)
        {
            return (Symbol)value.RefVal;
        }

        //        public static explicit operator Thread(Value value)
        //        {
        //            return (Thread)value.RefVal;
        //        }
        //
        //        public static explicit operator Callable(Value value)
        //        {
        //            if (value.RefVal != null && !Callable.IsCallable(value.RefVal))
        //                throw new InvalidCastException();
        //
        //            return new Callable() { Val = value.RefVal };
        //        }

        #endregion




    }



}
