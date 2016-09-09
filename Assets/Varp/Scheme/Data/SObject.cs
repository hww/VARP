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

using VARP.Scheme.Exception;
using VARP.Scheme.REPL;

namespace VARP.Scheme.Data
{

    public enum InspectOptions
    {
        Default,
        PrettyPrint
    }

    public class SObject
    {
        // this value have to be used everywhere in your code
        public static SVoid Void = new SVoid();

        public override string ToString()
        {
            return Inspect();
        }
        public virtual SBool AsBool() { return SBool.False; }
        public virtual string AsString() { return base.ToString(); }
        public virtual string Inspect() { return Inspector.Inspect(this); }

        public virtual bool IsNull { get { return false; } }
        public virtual bool IsVoid { get { return false; } }
        public virtual bool IsList { get { return false; } }
        public virtual bool IsPair { get { return false; } }
        public virtual bool IsLiteral { get { return true; } }
        public virtual bool IsIdentifier { get { return true; } }
        public virtual bool IsSymbol { get { return false; } }
        public virtual bool IsKeyword { get { return false; } }
        public virtual bool IsNumeric { get { return false; } }
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
            throw new ContractViolation(obj,"symbol?", Inspector.Inspect(obj));
        }
        // ---------------------------------------------------------------
        // Numerical
        // ---------------------------------------------------------------
        public static SInteger GetSInteger(SObject obj) {
            if (obj is SInteger)   
                return obj as SInteger;
            throw new ContractViolation(obj, "int?", Inspector.Inspect(obj));
        }
        public static SFloat GetFloat(SObject obj) {
            if (obj is SFloat)
                return obj as SFloat;
            throw new ContractViolation(obj, "bool?", Inspector.Inspect(obj));
        }
        // ---------------------------------------------------------------
        // Boolean
        // ---------------------------------------------------------------
        public static SBool GetBool(SObject obj) {
            if (obj is SBool)
                return obj as SBool;
            throw new ContractViolation(obj, "bool?", Inspector.Inspect(obj));
        }
        // ---------------------------------------------------------------
        // String
        // ---------------------------------------------------------------
        public static SString GetString(SObject obj) {
            if (obj is SString)
               return obj as SString;
            throw new ContractViolation(obj, "string?", Inspector.Inspect(obj));
        }
        public static SChar GetChar(SObject obj) {
            if (obj is SChar)
                return obj as SChar;
            throw new ContractViolation(obj, "char?", Inspector.Inspect(obj));
        }
        // ---------------------------------------------------------------
        // Pair
        // ---------------------------------------------------------------
        // produces exceptions if @obj is null or not Par
        public static Pair GetPair(SObject obj) {
            if (obj is Pair) return obj as Pair;
            throw new ContractViolation(obj, "cons?", Inspector.Inspect(obj));
        }
        // return null if @obj is null, exception if object is not Pair and not null
        public static Pair GetPairOrNull(SObject obj)
        {
            if (obj == null) return null;
            if (obj is Pair) return obj as Pair;
            throw new ContractViolation(obj, "cons?", Inspector.Inspect(obj));
        }
        // return null if @obj is null and Pair if object is list. 
        // exception for improper list
        public static Pair GetList(SObject obj) {
            if (obj == null) return null;
            if (obj.IsList) return obj as Pair;
            throw new ContractViolation(obj, "list?", Inspector.Inspect(obj));
        }
        public static SObject GetLiteral(SObject obj) {
            if(obj.IsLiteral)
                return obj;
            throw new ContractViolation(obj, "literal?", Inspector.Inspect(obj));
        }

        #endregion
    }
}
