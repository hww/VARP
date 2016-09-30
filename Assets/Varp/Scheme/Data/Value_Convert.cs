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
using System.Collections.Generic;

namespace VARP.Scheme.Data
{
    using DataStructures;
    using Stx;

    /// <summary>
    /// Value constructors
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct Value
    {
        #region Convert to

        /// <summary>
        /// Returns true if the value is non-nil and not false.
        /// </summary>
        public char AsChar()
        {
            return RefVal is CharClass ? Convert.ToChar(NumVal) : (char)0;
        }

        /// <summary>
        /// Returns true if the value is non-nil and not false.
        /// </summary>
        public bool AsBool()
        {
            return RefVal != null && RefVal != BoolClass.False;
        }

        /// <summary>
        /// Returns the numeric value (if it is a number, else zero).
        /// Does not perform string conversions.
        /// </summary>
        public double AsDouble()
        {
            return RefVal is NumericalClass ? (double)NumVal : 0;
        }

        /// <summary>
        /// Returns the numeric value (if it is a number, else zero).
        /// Rounds via truncation.
        /// Does not perform string conversions.
        /// </summary>
        public int AsInt32()
        {
            return RefVal is NumericalClass ? (int)NumVal : 0;
        }

        /// <summary>
        /// Returns the numeric value (if it is a number, else zero).
        /// Rounds via truncation.
        /// Does not perform string conversions.
        /// </summary>
        public uint AsUInt32()
        {
            return RefVal is NumericalClass ? (uint)NumVal : 0;
        }

        /// <summary>
        /// Returns the value as a string (or a nil string if the value
        /// is not a string).
        /// Does not perform string conversions.
        /// </summary>
        public string AsString()
        {
            return RefVal as string;
        }

        /// <summary>
        /// Convert to the Pair type
        /// </summary>
        /// <returns></returns>
        public LinkedList<Value> AsLinkedList<Value>()
        {
            return RefVal is LinkedList<Value> ? (LinkedList<Value>)RefVal : null;
        }

        /// <summary>
        /// Convert to the Pair type
        /// </summary>
        /// <returns></returns>
        public ValuePair AsValuePair()
        {
            return RefVal is ValuePair ? (ValuePair)RefVal : null;
        }

        /// <summary>
        /// Returns the value as a table (if it is a table, else returns null).
        /// </summary>
        public Dictionary<object, Value> AsTable()
        {
            return RefVal as Dictionary<object, Value>;
        }

        /// <summary>
        /// Convert to the Vector type
        /// </summary>
        /// <returns></returns>
        public List<T> AsList<T>()
        {
            return RefVal is List<T> ? (List<T>)RefVal : null;
        }

        /// <summary>
        /// Convert to the Symbol type
        /// </summary>
        /// <returns></returns>
        public Symbol AsSymbol()
        {
            return RefVal is Symbol ? (Symbol)RefVal : null;
        }

        /// <summary>
        /// Convert to the Syntax type
        /// </summary>
        /// <returns></returns>
        public Syntax AsSyntax()
        {
            return RefVal is Syntax ? (Syntax)RefVal : null;
        }


        /// <summary>
        /// Convert to the AST type
        /// </summary>
        /// <returns></returns>
        public AST AsAST()
        {
            return RefVal is AST ? (AST)RefVal : null;
        }

        /// <summary>
        /// Attempts to cast to a Table or UserData type,
        /// returning <c>null</c> on failure.
        /// </summary>
        public T As<T>() where T : class
        {
            var val = RefVal;

            if (val is NumericalClass || val is BoolClass)
                return null;

            return val as T;
        }

        #endregion

        /// <summary>
        /// TODO Can be used ValueClass virtual method to convert 
        /// ref Value to he string. It can eliminate the condition
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (RefVal == null)
                return "nil";
            if (RefVal is string)
                return RefVal as string;
            if (RefVal is INumeric)
                return (RefVal as INumeric).ToString(NumVal);
            if (RefVal is ValueClass)
                return (RefVal as ValueClass).ToString();
            if (RefVal is List<Value>)
                return ValueList.ToString(RefVal as List<Value>);
            if (RefVal is LinkedList<Value>)
                return ValueLinkedList.ToString(RefVal as LinkedList<Value>);
            if (RefVal is Dictionary<object, Value>)
                return ValueDictionary.ToString(RefVal as Dictionary<object, Value>);
            return RefVal.ToString();
        }

        #region DebuggerDisplay 
        public string DebuggerDisplay
        {
            get
            {
                try
                {
                    return string.Format("#<Value Ref={0} Num={1}>", RefVal == null ? "null" : RefVal, NumVal);
                }
                catch (System.Exception ex)
                {
                    return string.Format("#<Value Ref={0} Num={1} Err={2}>", RefVal == null ? "null" : RefVal.GetType().ToString(), NumVal, ex.Message);
                }

            }
        }
        #endregion
    }

}
