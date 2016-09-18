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


namespace VARP.Scheme.Data
{
    using Exception;
    using System.Diagnostics;


    /// <summary>
    /// Value constructors
    /// </summary>
    public partial struct Value
    {
        #region Set

        public void SetNil()
        {
            RefVal = null;
        }

        public void Set(bool value)
        {
            RefVal = value ? BoolClass.True : BoolClass.False;
        }

        public void Set(int value)
        {
            RefVal = FixnumClass.Instance;
            NumVal = value;
        }

        public void Set(uint value)
        {
            RefVal = FixnumClass.Instance;
            NumVal = value;
        }

        public void Set(double value)
        {
            RefVal = FloatClass.Instance;
            NumVal = value;
        }
        public void Set(string value)
        {
            RefVal = value;
            NumVal = 0;
        }

        public void Set(BoolClass value)
        {
            RefVal = value;
            NumVal = 0;
        }

        public void Set(ValueClass value)
        {
            if (value is NumericalClass)
                throw SchemeError.Error("value-set", "cant assign numeric value type", value);
            RefVal = value;
            NumVal = 0;
        }

        public void Set(Value value)
        {
            this = value;
        }

        public void Set(object value)
        {
            if (value == null)
            {
                // Nill value
                RefVal = null;
                NumVal = 0;
                return;
            }

            if (value is ValueClass)
            {
                if (value is NumericalClass)
                    throw SchemeError.Error("value-set", "cant assign numeric value type", value);
                if (value is BoolClass)
                    Set(value as BoolClass); //< will check possible options
                RefVal = value as ValueClass;
                NumVal = 0;
                return;
            }

            if (value.GetType().IsValueType)
            {
                // This is the value type case. 
                // it means the data is C# in the box
                SetFromValueType(value);
                return;
            }

            // This is the class type which is not ValueType
            RefVal = value;
        }

        /// <summary>
        /// Set value from the C# value type as the argument 
        /// is object reference then value is in the box.
        /// </summary>
        /// <param name="value"></param>
        private void SetFromValueType(object value)
        {
            // the quick check

            if (value is Value)
                this = (Value)value;

            // core types first

            else if (value is bool)
                Set((bool)value);
            else if (value is int)
                Set((int)value);
            else if (value is double)
                Set((double)value);

            //and now all the odd cases (note the leading else!)

            else if (value is uint)
                Set((double)(uint)value);
            else if (value is float)
                Set((double)(float)value);
            else if (value is sbyte)
                Set((double)(sbyte)value);
            else if (value is byte)
                Set((double)(byte)value);
            else if (value is short)
                Set((double)(short)value);
            else if (value is ushort)
                Set((double)(ushort)value);
            else
                throw SchemeError.Error("value-set", "can't assign the C# value-type", value);
        }

        #endregion

    }

}
