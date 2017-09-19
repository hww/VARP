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
    /// A more compact Value representation.
    /// This type boxes its numbers, and reuses the boxes to reduce GC thrashing.
    /// Do -NOT- blithely copy this type around, as it can cause issues with the boxed numbers.
    /// </summary>
    internal struct CompactValue
    {
        internal object RefVal;

        public CompactValue(Value value)
        {
            if (value.RefVal == global::NumericalClass.Float)
                RefVal = new NumBox(value.NumVal);
            else
                RefVal = value.RefVal;
        }

        public CompactValue(double value)
        {
            RefVal = new NumBox(value);
        }

        public CompactValue(ValueType value)
        {
            RefVal = value;
        }

        public void Set(ref Value value)
        {
            if (value.RefVal == global::NumericalClass.Float)
                Set(value.NumVal);
            else
                RefVal = value.RefVal;
        }

        public void Set(double value)
        {
            var asNum = RefVal as NumBox;
            if (asNum == null)
                RefVal = asNum = new NumBox();
            asNum.NumVal = value;
        }

        public double ToDouble()
        {
            var asNum = RefVal as NumBox;
            return asNum != null ? asNum.NumVal : 0;
        }

        public void ToValue(out Value v)
        {
            var asNum = RefVal as NumBox;
            if (asNum != null)
            {
                v.RefVal = global::NumericalClass.Float;
                v.NumVal = asNum.NumVal;
            }
            else
            {
                v.RefVal = RefVal;
                v.NumVal = 0;
            }
        }

        public Value ToValue()
        {
            Value ret;
            ToValue(out ret);
            return ret;
        }

        public static bool Equals(CompactValue a, CompactValue b)
        {
            if (a.RefVal == b.RefVal)
                return true;

            if (a.RefVal != null) return a.RefVal.Equals(b.RefVal);

            return false;
        }

        public bool Equals(CompactValue other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return obj is CompactValue && Equals((CompactValue)obj);
        }

        public bool Equals(Value value)
        {
            if (RefVal == value.RefVal)
                return true;

            if (value.RefVal is FloatType)
            {
                var asNum = RefVal as NumBox;
                return asNum != null && asNum.NumVal == value.NumVal;
            }

            return false;
        }

        public bool Equals(double value)
        {
            var asNum = RefVal as NumBox;
            return asNum != null && asNum.NumVal == value;
        }

        public bool Equals(ValueType value)
        {
            if (value == RefVal) return true;
            if (value == null) return false;
            return value.Equals(RefVal);
        }

        public override int GetHashCode()
        {
            if (RefVal == null || RefVal == global::NumericalClass.Float)
                return 0;

            if (RefVal == BoolType.True)
                return 1;

            var asNum = RefVal as NumBox;
            if (asNum != null)
                return Value.GetHashCode(asNum.NumVal);

            var asWrapper = RefVal as UserDataWrapper;
            if (asWrapper != null)
                Value.GetHashCode(asWrapper.GetHashCode());

            return Value.GetHashCode(RefVal);
        }

        public override string ToString()
        {
            return ToValue().ToString();
        }
    }

    /// <summary>
    /// Number type container
    /// </summary>
    internal sealed class NumBox : ValueType
    {
        public double NumVal;

        public NumBox()
        {
        }

        public NumBox(double value)
        {
            NumVal = value;
        }

        public override string ToString()
        {
            return NumVal.ToString();
        }
    }

    /// <summary>
    /// Container for the value references
    /// </summary>
    internal sealed class ValueBox : ValueType
    {
        public Value Value;

        public ValueBox()
        {
        }

        public ValueBox(Value value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// When you iterate over the elements of the array, 
    /// the first non-initialized index will result in nil; 
    /// you can use this value as a sentinel to represent 
    /// the end of the array. For instance, you could print 
    /// the lines read in the last example with the 
    /// following code:
    /// </summary>
    public class Sentinel : ValueType
    {
        public Sentinel(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return "Sentinel: " + Name;
        }
    }

    /// <summary>
    /// Pint to object which is not belongs to ValueClass
    /// </summary>
    internal class UserDataWrapper : ValueType
    {
        public object RefVal;

        public UserDataWrapper(object value)
        {
            Debug.Assert(value != null);
            RefVal = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode(RefVal);
        }
    }
}