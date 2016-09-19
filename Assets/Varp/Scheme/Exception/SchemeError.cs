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

using System.Text;
using System.Collections.Generic;

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
    public class SchemeError : System.ApplicationException
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

        /// ================================================================================
        /// Expand location string from object
        /// ================================================================================
       
        static protected string GetLocationString(object x)
        {
            if (x is Location)
                return GetLocationStringIntern(x as Location);
            if (x is Token)
                return GetLocationStringIntern((x as Token).location);
            if (x is Syntax)
                return GetLocationStringIntern((x as Syntax).Location);
            return string.Empty;
        }

        static string GetLocationStringIntern(Location x)
        {
            return x == null ? string.Empty : string.Format("{0}({1},{2}): ", x.File, x.LineNumber, x.ColNumber);
        }

        /// ================================================================================
        /// 
        /// All messages have to be down-cased
        /// Format:
        /// @srcloc: @name: @message; @continued-message ...  @field: @detail›
        /// 
        /// @srcloc             | location in source file
        /// @name               | name of the method where happen exception
        /// @message            | short message as "syntax error"
        /// @continuedMessage   | long error message, for several sentences separate by ';'
        /// 
        /// ================================================================================

        /// <summary>
        /// Create simple message without any formatting and inspect objects
        /// </summary>
        /// <param name="message">the mesage</param>
        /// <param name="fields">inspected objects</param>
        public static string ErrorMessage(string message, params object[] fields)
        {
            StringBuilder sb = new StringBuilder();
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

        /// <summary>
        /// Create message with source location, and additionally
        /// the the formatted string builder will be called
        /// Use ? formatter as:
        /// string.Format("{0,?}", obj);
        /// </summary>
        /// <param name="message">the mesage</param>
        /// <param name="fields">inspected objects</param>
        public static string ErrorMessage(string name, string format, params object[] fields)
        {
            return string.Format(new SchemeFormatter(), "{0}: {1}", name, string.Format(format, fields));
        }
        public static SchemeError Error(string name, string expected, params object[] fields)
        {
            return new SchemeError(ErrorMessage(name, expected, fields));
        }

        // ----------------------
        // Argument error methods
        // ----------------------

        public static string ArgumentErrorMessage(string name, string expected, object val)
        {
            string vstr = Inspect(val);
            return string.Format("{0}: contract violation\n   expected: {1}\n   given: {2}", name, expected, vstr);
        }

        public static string ArgumentErrorMessage(string name, string expected, int badPos, params object[] vals)
        {
            StringBuilder sb = new StringBuilder();
            string loc = string.Empty;
            string badStr = string.Empty;
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
            string argStr = sb.ToString();

            return string.Format("{0}{1}: contract violation\n   expected: {2}\n   given: {3}\n  argument position: {4}\n  other arguments...:\n{5}", loc, name, expected, badPos, badStr, argStr);
        }

        public static string ArgumentErrorMessage(string name, string expected, int badPos, LinkedList<Value> vals)
        {
            Value[] array = new Value[vals.Count];
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


        // ----------------------
        // Result error methods
        // ----------------------

        public static string ResultErrorMessage(string name, string expected, object val)
        {
            string locs = GetLocationString(val);
            string vstr = Inspect(val);
            return string.Format("{0}{1}: contract violation\n   expected: {2}\n   given: {3}", locs, name, expected, vstr);
        }

        public static string ResultErrorMessage(string name, string expected, int badPos, params object[] vals)
        {
            StringBuilder sb = new StringBuilder();
            string loc = string.Empty;
            string badStr = string.Empty;
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
            string argStr = sb.ToString();
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

        // ----------------------
        // Range error methods
        // ----------------------

        public static string RangeErrorMessage(string name,       // "vector-ref" | "array-ref"
                                        string typeDescription,   // "vector" | "array"
                                        string indexPrefix,       // "start"
                                        int index,                // current index
                                        object inValue,           // [1,2,3,4,5,6]
                                        int lowerBound,           // minimum index
                                        int upperBound)           // maximum index
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);
            sb.Append(": ");
            sb.Append(indexPrefix);

            string msg = string.Empty;
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

        // ----------------------
        // Arity error methods
        // ----------------------

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
            StringBuilder sb = new StringBuilder();
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

        public static SchemeError ArityError(string name, string message, int expected, int given, LinkedList<Value> argv, Syntax expression)
        {
            return new SchemeError(ArityErrorMessage(name, message, expected, given, argv, expression));
        }


        // ----------------------
        // Syntax error methods
        // ----------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">name of method where happen error</param>
        /// <param name="message">error message</param>
        /// <param name="expression">expression where happen error</param>
        /// <param name="subexpression">exact token or syntax where happen error</param>
        /// <returns></returns>
        public static string SyntaxErrorMessage(string name, string message, object expression, object subexpression = null)
        {
            string expressionStr = Inspect(expression);
            if (subexpression == null)
            {
                string loc = GetLocationString(expression);
                return string.Format("{0}: {1}: {2} in: {3}", loc, name, message, expressionStr);
            }
            else
            {
                string loc = GetLocationString(subexpression);
                string subexpressionStr = Inspect(expression);
                return string.Format("{0}: {1}: {2} in: {3}\n error syntax: {4}", loc, name, message, expressionStr);
            }
        }

        public static SchemeError SyntaxError(string name, string message, object expression, object subexpression = null)
        {
            return new SchemeError(SyntaxErrorMessage(name, message, expression, subexpression));
        }

        // ----------------------
        // Inspector
        // ----------------------

        // Standart REPL inspector uses o.Inspect() method
        // this verio will use AsStrin() method

        /// <summary>
        /// Inspect object for error message
        /// </summary>
        /// <param name="o"></param>
        static string Inspect(object o)
        {
            if (o == null)
                return "()";
            if (o is Value)
                return ((Value)o).AsString();
            if (o is ValueClass)
                return (o as ValueClass).ToString();
            return o.ToString();
        }
    }
}