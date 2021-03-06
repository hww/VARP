﻿/* 
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

using System.Text;
using System.Collections.Generic;


// ================================================================================
// 
// All messages have to be down-cased
// Format:
// @srcloc: @name: @message; @continued-message ...  @field: @detail›
// 
// @srcloc             | location in source file
// @name               | name of the method where happen exception
// @message            | short message as "syntax error"
// @continuedMessage   | long error message, for several sentences separate by ';'
// 
// ================================================================================

namespace VARP.Scheme.Exception
{
    using DataStructures;
    using Tokenizing;
    using Stx;
    using REPL;
    using Data;


    /// <summary>
    /// General exception class
    /// </summary>
    public partial class SchemeError : System.ApplicationException
    {
        public SchemeError() : base()
        {
        }

        public SchemeError(string message) : base(message)
        {
        }

        public SchemeError(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        // ================================================================================
        // Special formatter
        // ================================================================================

        /// <summary>
        /// Create message with source location, and additionally
        /// the the formatted string builder will be called
        /// </summary>
        /// <usage>
        /// string.Format("{0,?}", obj);
        /// </usage>
        /// <param name="format"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public string Format(string format, params object[] objects)
        {
            return string.Format(new SchemeFormatter(), format, objects);
        }

        // ================================================================================
        // Expand location string from object
        // ================================================================================

        protected static string GetLocationString(object x)
        {
            if (x is Location)
                return GetLocationStringIntern(x as Location);
            if (x is Token)
                return GetLocationStringIntern((x as Token).location);
            if (x is Syntax)
                return GetLocationStringIntern((x as Syntax).Location);
            return string.Empty;
        }


        protected static string GetLocationStringIntern(Location x)
        {
            return x == null ? string.Empty : string.Format("{0}({1},{2}): ", x.File, x.LineNumber, x.ColNumber);
        }

        // ================================================================================
        // Inspector
        // ================================================================================

        /// <summary>
        /// Inspect object for error message
        /// Standart REPL inspector uses o.Inspect() method
        /// this version will use AsString() method
        /// </summary>
        /// <param name="o"></param>
        protected static string Inspect(object o)
        {
            if (o == null)
                return "()";
            if (o is Value)
                return ((Value)o).AsString();
            if (o is ValueType)
                return (o as ValueType).ToString();
            return o.ToString();
        }
    }

    public partial class SchemeError
    {
        /// <summary>
        /// Create simple message without any formatting and inspect objects
        /// </summary>
        /// <param name="message">the mesage</param>
        /// <param name="fields">inspected objects</param>
        public static string ErrorMessage(string message, params object[] fields)
        {
            var sb = new StringBuilder();
            sb.Append(message);
            sb.Append(": ");
            foreach (var v in fields)
            {
                sb.Append(" ");
                sb.Append(Inspect(v));
            }
            return sb.ToString();
        }

        public static SchemeError Error(string message, params object[] fields)
        {
            return new SchemeError(ErrorMessage(message, fields));
        }

        public static string ErrorMessageWithName(string name, string message, params object[] fields)
        {
            return ErrorMessage(string.Format("{0}: {1}", name, message), fields);
        }

        public static SchemeError ErrorWithName(string name, string message, params object[] fields)
        {
            return new SchemeError(ErrorMessageWithName(name, message, fields));
        }
    }

    /// <summary>
    /// Argument error methods
    /// </summary>
    public partial class SchemeError
    {
        public static string ArgumentErrorMessage(string name, string expected, object val)
        {
            var vstr = Inspect(val);
            return string.Format("{0}: contract violation\n   expected: {1}\n   given: {2}", name, expected, vstr);
        }

        public static string ArgumentErrorMessage(string name, string expected, int badPos, params object[] vals)
        {
            var sb = new StringBuilder();
            var loc = string.Empty;
            var badStr = string.Empty;
            for (var i = 0; i < vals.Length; i++)
            {
                if (i == badPos)
                {
                    loc = GetLocationString(vals[i]);
                    badStr = Inspect(vals[i]);
                    continue; // skip bad argument
                }
                sb.Append("  ");
                sb.AppendLine(Inspect(vals[i]));
            }
            var argStr = sb.ToString();

            return string.Format("{0}{1}: contract violation\n   expected: {2}\n   given: {3}\n  argument position: {4}\n  other arguments...:\n{5}", loc, name, expected, badPos, badStr, argStr);
        }

        public static string ArgumentErrorMessage(string name, string expected, int badPos, LinkedList<Value> vals)
        {
            var array = new Value[vals.Count];
            vals.CopyTo(array, 0);
            return ArgumentErrorMessage(name, expected, badPos, array);
        }

        public static SchemeError ArgumentError(string name, string expected, object val)
        {
            return new SchemeError(ResultErrorMessage(name, expected, val));
        }

        public static SchemeError ArgumentError(string name, string expected, int badPos, params object[] vals)
        {
            return new SchemeError(ResultErrorMessage(name, expected, badPos, vals));
        }

        public static SchemeError ArgumentError(string name, string expected, int badPos, LinkedList<Value> val)
        {
            return new SchemeError(ResultErrorMessage(name, expected, val));
        }
    }

    /// <summary>
    /// Result error methods
    /// </summary>
    public partial class SchemeError
    {
        /// <summary>
        /// Result error message for single result outputs
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expected"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string ResultErrorMessage(string name, string expected, object result)
        {
            var locs = GetLocationString(result);
            var vstr = Inspect(result);
            return string.Format("{0}{1}: contract violation\n   expected: {2}\n   given: {3}", locs, name, expected, vstr);
        }

        /// <summary>
        /// Result error message for multiple results
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expected"></param>
        /// <param name="badPos"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static string ResultErrorMessage(string name, string expected, int badPos, params object[] results)
        {
            var sb = new StringBuilder();
            var loc = string.Empty;
            var badStr = string.Empty;
            for (var i = 0; i < results.Length; i++)
            {
                if (i == badPos)
                {
                    loc = GetLocationString(results[i]);
                    badStr = Inspect(results[i]);
                    continue; // skip bad argument
                }
                sb.Append("  ");
                sb.AppendLine(Inspect(results[i]));
            }
            var argStr = sb.ToString();
            return string.Format("{0}{1}: contract violation\n   expected: {2}\n   given: {3}\n  result position: {4}\n  other result position...:\n{5}", loc, name, expected, badPos, badStr, argStr);
        }

        public static SchemeError ResultError(string name, string expected, object val)
        {
            return new SchemeError(ResultErrorMessage(name, expected, val));
        }

        public static SchemeError ResultError(string name, string expected, int badPos, params object[] vals)
        {
            return new SchemeError(ResultErrorMessage(name, expected, badPos, vals));
        }
    }

    /// <summary>
    /// Range error methods
    /// </summary>
    public partial class SchemeError
    {
        /// <summary>
        /// Range error message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeDescription"></param>
        /// <param name="indexPrefix"></param>
        /// <param name="index"></param>
        /// <param name="inValue"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static string RangeErrorMessage(string name,       //< "vector-ref" | "array-ref"
                                        string typeDescription,   //< "vector" | "array"
                                        string indexPrefix,       //< "start"
                                        int index,                //< current index
                                        object inValue,           //< [1,2,3,4,5,6]
                                        int lowerBound,           //< minimum index
                                        int upperBound)           //< maximum index
        {
            var sb = new StringBuilder();
            sb.Append(name);
            sb.Append(": ");
            sb.Append(indexPrefix);

            var msg = string.Empty;
            if (index < lowerBound)
                sb.Append(" index is out of range\n");
            else if (index > upperBound)
                sb.Append(" index is out of range\n");
            else
                throw new SchemeError("Bad arguments");

            sb.Append(string.Format("  {0} index: {1}\n", indexPrefix, index));
            sb.Append(string.Format(" valid-range: [{0},{1}]\n", lowerBound, upperBound));
            sb.Append(string.Format(" {0}: \n  ", typeDescription));
            sb.Append(Inspect(inValue));

            return sb.ToString();
        }

        /// <summary>
        /// Range exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeDescription"></param>
        /// <param name="indexPrefix"></param>
        /// <param name="index"></param>
        /// <param name="inValue"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static SchemeError RangeError(
            string name,                            // "vector-ref" | "array-ref"
            string typeDescription,                 // "vector" | "array"
            string indexPrefix,                     // "start"
            int index,                              // current index
            object inValue,                         // [1,2,3,4,5,6]
            int lowerBound,                         // minimum index
            int upperBound)                         // maximum index
        {
            return new SchemeError(RangeErrorMessage(name, typeDescription, indexPrefix, index, inValue, lowerBound, upperBound));
        }
    }
    /// <summary>
    /// Arity error methods
    /// </summary>
    public partial class SchemeError
    {
        /// <summary>
        /// Arity error message
        /// </summary>
        /// <param name="name">function name wehre happens error</param>
        /// <param name="message">the error message</param>
        /// <param name="expected">expected arguments quantity</param>
        /// <param name="given">given arguments quantity</param>
        /// <param name="argv">arguments</param>
        /// <param name="expression">the expression whenre happens error</param>
        /// <returns></returns>
        public static string ArityErrorMessage(string name, string message, int expected, int given, LinkedList<Value> argv, Syntax expression)
        {
            var sb = new StringBuilder();
            sb.Append(GetLocationString(expression));
            sb.Append("arity mismatch;\n");
            sb.Append("  the expected number of arguments does not match the given number\n");
            sb.Append(string.Format("  expected: {0}\n", expected));
            sb.Append(string.Format("  given: {0}\n", given));
            sb.Append("  arguments...:\n");
            foreach (var arg in argv)
                sb.AppendLine("  " + Inspect(arg));
            return sb.ToString();
        }

        /// <summary>
        /// Arity exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="expected"></param>
        /// <param name="given"></param>
        /// <param name="argv"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SchemeError ArityError(string name, string message, int expected, int given, LinkedList<Value> argv, Syntax expression)
        {
            return new SchemeError(ArityErrorMessage(name, message, expected, given, argv, expression));
        }
    }

    /// <summary>
    /// Syntax error methods
    /// </summary>
    public partial class SchemeError 
    { 
        /// <summary>
        /// Syntax error message
        /// </summary>
        /// <param name="name">name of method where happen error</param>
        /// <param name="message">error message</param>
        /// <param name="expression">expression where happen error</param>
        /// <param name="subexpression">exact token or syntax where happen error</param>
        /// <returns></returns>
        public static string SyntaxErrorMessage(string name, string message, object expression, object subexpression = null)
        {
            if (subexpression == null)
            {
                var expStr = Inspect(expression);
                var expLoc = GetLocationString(expression);
                return string.Format("{0}: {1}: {2} in: {3}", expLoc, name, message, expStr);
            }
            else
            {
                var expStr = Inspect(expression);
                var subLoc = GetLocationString(subexpression);
                var subStr = Inspect(expression);
                return string.Format("{0}: {1}: {2} in: {3}\n error syntax: {4}", subLoc, name, message, expStr, subStr);
            }
        }

        /// <summary>
        /// Create new syntax error exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="expression"></param>
        /// <param name="subexpression"></param>
        /// <returns></returns>
        public static SchemeError SyntaxError(string name, string message, object expression, object subexpression = null)
        {
            return new SchemeError(SyntaxErrorMessage(name, message, expression, subexpression));
        }
    }

}