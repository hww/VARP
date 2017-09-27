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

using System.Collections.Generic;
using System.Diagnostics;

namespace VARP.Scheme.Data
{
    using Exception;
    using REPL;
    using Stx;
    using DataStructures;



    /// <summary>
    /// The Value object is sort of variative container. It has two fields: 
    /// 
    ///     internal double NumVal;     Numerical value
    ///     internal object RefVal;     Referenced value
    ///     
    /// In case if the value contains: integer, boolean, float, character, etc.
    /// the NumVal contains the value of variable. But the reference point
    /// to the ValueType object. This object designate the type of data 
    /// located in the NumVal
    /// 
    /// The class ValueType have to be used as parent class for
    /// any value type.
    /// </summary>
    public abstract class ValueType
    {
        /// <summary>
        /// When upvalue reffer to this tag the numeric part has index
        /// of variable. 
        /// </summary>
        internal static readonly UpvalueType OpenUpValueTag = new UpvalueType();

        public virtual bool AsBool() { return false; }
        public override string ToString() { return base.ToString(); }
        public virtual string Inspect() { return Inspector.Inspect(this); }

    }

    // =================================================================================
    // Classes for the types which does not need
    // reference inside the Value structure.
    // Each this class have also statically allocated 
    // object of this class.
    // When Value structure contains the Nill, Void, Number or Bool
    // it's reference pointer point to that static object
    // =================================================================================

    /// <summary>
    /// The value is UpValue of VM.  
    /// </summary>
    public class UpvalueType : ValueType
    {
    }

    /// <summary>
    /// When Nill type the numeric part does not mater
    /// </summary>
    //public sealed class NilType : ValueType {
    //    public static readonly NilType Nil = new NilType();
    //    public override string ToString() { return "nil"; }
    //    public override bool AsBool() { return false; }
    //}

    /// <summary>
    /// When Boolean type the numeric part does not mater
    /// </summary>
    public abstract class BoolType : ValueType {
        public static readonly VARP.Scheme.Data.BoolType True = new TrueType();
        public static readonly VARP.Scheme.Data.BoolType False = new FalseType();
    }

    /// <summary>
    /// True class
    /// </summary>
    public sealed class TrueType : BoolType {
        public override string ToString() { return "#t"; }
        public override bool AsBool() { return true; }
    }

    /// <summary>
    /// False class
    /// </summary>
    public sealed class FalseType : BoolType {
        public override string ToString() { return "#f"; }
        public override bool AsBool() { return false; }
    }

    /// <summary>
    /// When Numeric type the numeric part contains the value
    /// </summary>
    public abstract class NumericalType : ValueType
    {
        public static readonly FloatType Float = new FloatType();
        public static readonly FixnumType Fixnum = new FixnumType();
    }

    /// <summary>
    /// This Value class uses double value for containing
    /// current scalar value. 
    /// </summary>
    public interface INumeric
    {
        /// <summary>
        /// Convert scalar value to the string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ToString(double value);
    }

    /// <summary>
    /// When Float type the numeric part contains the value
    /// </summary>
    public sealed class FloatType : NumericalType, INumeric
    {
        public string ToString(double value) { return Convert.ToSingle(value).ToString("0.0##############"); }
    }

    /// <summary>
    /// When FixNum type the numeric part contains the value
    /// </summary>
    public sealed class FixnumType : NumericalType, INumeric
    {
        public string ToString(double value) { return Convert.ToInt32(value).ToString(); }
    }

    /// <summary>
    /// This value can be used when C# function have to return scheme object
    /// But there is nothing to return and there can't be used NULL as returns
    /// too
    ///
    ///     Example if C# implementation of the function 'display'. In the Lisp
    ///     expression looks like (display 1)
    ///
    ///     ValueClass display(...)
    ///     {
    ///        // Do display value
    ///        return SVoid.VoidValue
    ///     }
    ///
    /// Difference between SVoid and SNull: SVoid can be recognized as empty list
    /// As it can be with SNull
    /// </summary>
    public sealed class VoidType : ValueType {
        public override string ToString() { return "void"; }
        public override bool AsBool() { return false; }
    }

    /// <summary>
    /// Types system of the Value structure
    /// </summary>
    public partial struct Value
    {
        internal static readonly Value Nil = new Value(null);
        internal static readonly Value Void = new Value(global::VoidType.Void);
        internal static readonly Value False = new Value(BoolType.False);
        internal static readonly Value True = new Value(BoolType.True);
        internal static readonly Value Lambda = new Value(Symbol.LAMBDA);



        /// <summary>
        /// Get type of the object. 
        /// </summary>
        //public ValueType Type
        //{
        //    get {
        //        if (RefVal == null)
        //            return ValueType.Nil;
        //        if (RefVal: RefVal as ValueType; }
        //}

        public bool Is<T>() { return RefVal is T; }
        public bool IsNil { get { return RefVal == null; } }
        public bool IsNotNil { get { return RefVal != null; } }
        public bool IsVoid { get { return RefVal is VoidType; } }
        public bool IsValueType { get { return RefVal is ValueType; } }
        public bool IsBool { get { return RefVal is BoolType; } }
        public bool IsTrue { get { return RefVal is TrueType; } }
        public bool IsFalse { get { return RefVal is FalseType; } }
        public bool IsNumber { get { return RefVal is NumericalType; } }
        public bool IsFixnum { get { return RefVal is FixnumType; } }
        public bool IsFloat { get { return RefVal is FloatType; } }
        public bool IsString { get { return RefVal is string; } }
        public bool IsSymbol { get { return RefVal is Symbol; } }
        public bool IsLinkedList<T>() { return RefVal is LinkedList<T>; }
        public bool IsTable { get { return RefVal is Dictionary<object, Value>; } }
        public bool IsList<T>() { return RefVal is List<T>; } 
        public bool IsSyntax { get { return RefVal is Syntax; } }
        public bool IsAst { get { return RefVal is AST; } }
        public bool IsValuePair { get { return RefVal is ValuePair; } }
    }

}
