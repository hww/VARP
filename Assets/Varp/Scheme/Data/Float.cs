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
    public class SFloat : SObject
    {
        public float Value;
        public SFloat() { }
        public SFloat(float val) { Value = val; }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj)
        {
            if (obj is SInteger) return Equals(obj as SInteger);
            if (obj is SFloat) return Equals(obj as SFloat);
            if (obj is int) return Equals((int)obj);
            return false;
        }
        public bool Equals(SInteger obj)
        {
            if (obj == null) return false;
            return obj.Value == Value;
        }
        public bool Equals(SFloat obj)
        {
            if (obj == null) return false;
            return obj.Value == Value;
        }
        public bool Equals(int obj)
        {
            return obj == Value;
        }


        #region SObject Methods
        public override SBool AsBool() { return Value==0 ? SBool.False : SBool.True; }
        public override string AsString() { return Value.ToString("0.0###############"); }
        public override bool IsNumeric { get { return true; } }
        #endregion

        #region Casting

        public static implicit operator float(SFloat d)
        {
            return d.Value;
        }
        public static implicit operator SFloat(int d)
        {
            return new SFloat(d);
        }

        #endregion
    }
}
