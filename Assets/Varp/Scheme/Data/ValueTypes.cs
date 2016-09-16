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
using VARP.Scheme.REPL;
using VARP.Scheme.Stx;

namespace VARP.Scheme.Data
{
    public interface INumeric
    {
        string ToString(double value);
    }

    /// <summary>
    /// Any object reference by the Value have to be inherit from this class
    /// </summary>
    public class ValueClass
    {
        public ValueClass()
        {
        }

        public override string ToString() { return Inspect(); }
        public virtual bool AsBool() { return false; }
        public virtual string AsString() { return base.ToString(); }
        public virtual string Inspect() { return Inspector.Inspect(this); }

        public Value ToValue()
        {
            // N.B. will make exception for numerical
            // because ValueClass does not have numerical
            // info
            return new Value(this); 
        }
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
    /// When Nill type the numeric part does not mater
    /// </summary>
    public sealed class NillClass : ValueClass {
        public static readonly NillClass Instance = new NillClass();
        public override string AsString() { return "()"; }
        public override bool AsBool() { return false; }
    }

    /// <summary>
    /// When Boolean type the numeric part does not mater
    /// </summary>
    public class BoolClass : ValueClass {
        public static readonly BoolClass True = new TrueClass();
        public static readonly BoolClass False = new FalseClass();
    }

    public sealed class TrueClass : BoolClass {
        public override string AsString() { return "#f"; }
        public override bool AsBool() { return true; }

    }
    public sealed class FalseClass : BoolClass {
        public override string AsString() { return "#f"; }
        public override bool AsBool() { return false; }
    }

    /// <summary>
    /// When Numeric type the numeric part contains the value
    /// </summary>
    public class NumberClass : ValueClass {}

    /// <summary>
    /// When Float type the numeric part contains the value
    /// </summary>
    public sealed class FloatClass : NumberClass, INumeric
    {
        public static readonly FloatClass Instance = new FloatClass();
        public string ToString(double value) { return Convert.ToSingle(value).ToString("0.0##############"); }
    }

    /// <summary>
    /// When FixNum type the numeric part contains the value
    /// </summary>
    public sealed class FixnumClass : NumberClass, INumeric
    {
        public static readonly FixnumClass Instance = new FixnumClass();
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
    public sealed class VoidClass : ValueClass {
        public static readonly VoidClass Instance = new VoidClass();
        public override string AsString() { return "void"; }
        public override bool AsBool() { return false; }
    }

    /// <summary>
    /// Types system of the Value structure
    /// </summary>
    public partial struct Value
    {
        public static readonly Value Nill = new Value(NillClass.Instance);
        public static readonly Value Void = new Value(VoidClass.Instance);
        public static readonly Value False = new Value(false);
        public static readonly Value True = new Value(true);

        /// <summary>
        /// Open up-values (and these should only be seen in up-value
        /// arrays in closures!) are represented with RefVal == OpenUpValueTag
        /// and NumVal == the stack index it points to.
        /// </summary>
        internal static readonly object OpenUpValueTag = new object();

        /// <summary>
        /// Get type of the object. 
        /// </summary>
        public ValueClass Type
        {
            get
            {
                if (RefVal == null)
                    return NillClass.Instance;
                return RefVal as ValueClass;
            }
        }

        public bool Is<T>() { return RefVal is T; }
        public bool IsNil { get { return RefVal == null; } }
        public bool IsNotNil { get { return RefVal != null; } }
        public bool IsBool { get { return RefVal is BoolClass; } }
        public bool IsTrue { get { return RefVal is TrueClass; } }
        public bool IsFalse { get { return RefVal is FalseClass; } }
        public bool IsNumber { get { return RefVal is NumberClass; } }
        public bool IsFixnum { get { return RefVal is FixnumClass; } }
        public bool IsFloat { get { return RefVal is FloatClass; } }
        public bool IsString { get { return RefVal is string; } }
        public bool IsValueTable { get { return RefVal is ValueTable; } }
        public bool IsSymbol { get { return RefVal is Symbol; } }
        public bool IsValueList { get { return RefVal is ValueList; } }
        public bool IsValueVector { get { return RefVal is ValueVector; } }
        public bool IsSyntax { get { return RefVal is Syntax; } }
        public bool IsAST { get { return RefVal is AST; } }
    }

}
