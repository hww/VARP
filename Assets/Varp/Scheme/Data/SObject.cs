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
    using REPL;

    public class SObject 
    {

        public override string ToString()
        {
            return Inspect();
        }
        public virtual SBool AsBool() { return SBool.False; }
        public virtual string AsString() { return base.ToString(); }
        public virtual string Inspect() { return Inspector.Inspect(this); }
        // self computed value
        public virtual bool IsLiteral { get { return false; } }
        // foo, bar, baz
        public virtual bool IsIdentifier { get { return false; } }
        // 0, 0.1, 99
        public virtual bool IsNumeric { get { return false; } }
        public virtual SObject GetDatum() { return this; }
    }

    public class SObjectCastSafe 
    {
        #region Cast to type 

        // This methods will produce exception if type is not required

        // ---------------------------------------------------------------
        // Symbolic
        // ---------------------------------------------------------------
        public static Symbol GetSymbol(SObject obj, Symbol def)
        {
            if (obj is Symbol)
                return (obj as Symbol);
            return def;
        }
        // ---------------------------------------------------------------
        // Numerical
        // ---------------------------------------------------------------
        public static SInteger GetSInteger(SObject obj, SInteger def)
        {
            if (obj is SInteger) 
                return obj as SInteger;
            return def;
        }
        public static SFloat GetFloat(SObject obj, SFloat def)
        {
            if (obj is SFloat)
                return obj as SFloat;
            return def;
        }
        // ---------------------------------------------------------------
        // Boolean
        // ---------------------------------------------------------------
        public static SBool GetBool(SObject obj, SBool def)
        {
            if (obj is SBool)
                return obj as SBool;
            return def;
        }
        // ---------------------------------------------------------------
        // String
        // ---------------------------------------------------------------
        public static SString GetString(SObject obj, SString def)
        {
            if (obj is SString)
                return obj as SString;
            return def;
        }
        public static SChar GetChar(SObject obj, SChar def)
        {
            if (obj is SChar)
                return obj as SChar;
            return def;
        }
        // ---------------------------------------------------------------
        // Pair
        // ---------------------------------------------------------------
        public static Pair GetCons(SObject obj, Pair def)
        {
            if (obj is Pair) return obj as Pair;
            return def;
        }
        public static Pair GetList(SObject obj, Pair def)
        {
            if (obj is Pair && (obj as Pair).Cdr is Pair)
                return obj as Pair;
            return def;
        }

        #endregion

    }

    public class SObjectCast 
    { 
        #region Cast to type 

        // This methods will produce exception if type is not required

        // ---------------------------------------------------------------
        // Symbolic
        // ---------------------------------------------------------------
        public static Symbol GetSymbol(SObject obj) {
            if (obj is Symbol)
                return (obj as Symbol);
            throw SchemeError.ArgumentError("get-symbol","symbol?", obj);
        }
        // ---------------------------------------------------------------
        // Numerical
        // ---------------------------------------------------------------
        public static SInteger GetSInteger(SObject obj) {
            if (obj is SInteger)   
                return obj as SInteger;
            throw SchemeError.ArgumentError("get-integer", "int?", obj);
        }
        public static SFloat GetFloat(SObject obj) {
            if (obj is SFloat)
                return obj as SFloat;
            throw SchemeError.ArgumentError("get-float", "bool?", obj);
        }
        // ---------------------------------------------------------------
        // Boolean
        // ---------------------------------------------------------------
        public static SBool GetBool(SObject obj) {
            if (obj is SBool)
                return obj as SBool;
            throw SchemeError.ArgumentError("get-bool", "bool?", obj);
        }
        // ---------------------------------------------------------------
        // String
        // ---------------------------------------------------------------
        public static SString GetString(SObject obj) {
            if (obj is SString)
               return obj as SString;
            throw SchemeError.ArgumentError("get-string", "string?", obj);
        }
        public static SChar GetChar(SObject obj) {
            if (obj is SChar)
                return obj as SChar;
            throw SchemeError.ArgumentError("get-char", "char?", obj);
        }
        // ---------------------------------------------------------------
        // Pair
        // ---------------------------------------------------------------
        // produces exceptions if @obj is null or not Par
        public static Pair GetPair(SObject obj) {
            if (obj is Pair) return obj as Pair;
            throw SchemeError.ArgumentError("get_pair", "cons?", obj);
        }
        // return null if @obj is null, exception if object is not Pair and not null
        public static Pair GetPairOrNull(SObject obj)
        {
            if (obj == null) return null;
            if (obj is Pair) return obj as Pair;
            throw SchemeError.ArgumentError("get-pair-or-null", "cons?", obj);
        }

        public static SObject GetLiteral(SObject obj) {
            if(obj.IsLiteral)
                return obj;
            throw SchemeError.ArgumentError("get-literal", "literal?", obj);
        }

        #endregion
    }
}
