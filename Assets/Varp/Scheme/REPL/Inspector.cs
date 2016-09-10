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
using System.Text;
using System.Globalization;
using VARP.Scheme.Data;
using VARP.Scheme.Stx;
using VARP.Scheme.Tokenizing;

namespace VARP.Scheme.REPL
{
    public interface Inspectable
    {
        string Inspect();
    }

    public enum InspectOptions
    {
        Default,
        PrettyPrint
    }

    /// <summary>
    /// Formats and prints objects
    /// </summary>
    public static class Inspector
    {
        public const int MAX_ARRAY_PRINT_LEN = 20;
        public const int MAX_CONS_PRINT_LEN = 20;

        public static string Inspect(Object x, InspectOptions options = InspectOptions.Default)
        {
            if (x == null)
                return "()";

            if (x is SObject)
                return InspectSchemeObject(x as SObject);

            if (x is Inspectable)
                return (x as Inspectable).Inspect();
            return InspectMonoObject(x, options);
        }

        static string InspectSchemeObject(SObject x, InspectOptions options = InspectOptions.Default)
        {
            if (x is Location)
                return InspectSchemeObjectIntern(x as Location, options);
            if (x is Token)
                return InspectSchemeObjectIntern(x as Token, options);
            if (x is Pair)
                return InspectSchemeObjectIntern(x as Pair);
            if (x is Syntax)
                return InspectSchemeObjectIntern(x as Syntax);
            if (x is SString)
                return string.Format("\"{0}\"", x.AsString());
            if (x is SVector)
                return InspectSchemeObjectIntern(x as SVector, options);
            if (x is AST)
                return (x as AST).Inspect();
            if (x is LexicalBinding)
                return InspectSchemeObjectIntern(x as LexicalBinding, options);
            if (x is LexicalEnvironment)
                return InspectSchemeObjectIntern(x as LexicalEnvironment, options);
            // all another just convert to string
            return x.AsString();

        }

        static string InspectSchemeObjectIntern(Location x, InspectOptions options = InspectOptions.Default)
        {
            return string.Format("#<location:{0}:{1} {2}>", x.LineNumber, x.ColNumber, x.File);
        }
        static string InspectSchemeObjectIntern(Token x, InspectOptions options = InspectOptions.Default)
        {
            Location loc = x.location;
            if (loc == null)
                return string.Format("#<token \"{0}\">", x.AsString());
            else
                return string.Format("#<token:{0}:{1} \"{2}\">", loc.LineNumber, loc.ColNumber, x.AsString());
        }
        static string InspectSchemeObjectIntern(Syntax x, InspectOptions options = InspectOptions.Default)
        {
            Location loc = x.location;
            if (loc == null)
                return string.Format("#<syntax {0}>", x.AsString());
            else
                return string.Format("#<syntax:{0}:{1} {2}>", loc.LineNumber, loc.ColNumber, Inspect(x.GetDatum(), options));
        }
        static string InspectSchemeObjectIntern(LexicalBinding bind, InspectOptions options = InspectOptions.Default)
        {
            string prefix = bind.IsPrimitive ? "#Prim" : string.Empty; 
                if (bind.IsGlobal)
                    return string.Format("{0} {1}", prefix, bind.Identifier.Name);
                else
                    return string.Format("[{0}] {1} {2}>", bind.Index, prefix, bind.Identifier.Name);
        }

        static string InspectSchemeObjectIntern(LexicalEnvironment env, InspectOptions options = InspectOptions.Default)
        {
            int tabs = env.GetLevelNumber();
            string tabstr = new string(' ', tabs * 4);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tabstr + "Lexical Environment");
            foreach (var b in env)
            {
                sb.AppendLine(tabstr + InspectSchemeObjectIntern(b, options));
            }
            foreach (var c in env.Children)
            {
                sb.AppendLine(tabstr + InspectSchemeObjectIntern(c as LexicalEnvironment, options));
            }

            return sb.ToString();
        }
        static bool IsSpecialForm(SObject obj)
        {
            if (obj is Symbol)
            {
                Symbol x = obj as Symbol;
                return x == Symbol.QUOTE || x == Symbol.QUASIQUOTE || x == Symbol.UNQUOTE || x == Symbol.UNQUOTESPLICE;
            }
            return false;
        }
        static string InspectSpecialReaderForm(Symbol x, InspectOptions options = InspectOptions.Default)
        {
            if (options == InspectOptions.PrettyPrint)
            {
                if (x == Symbol.QUOTE)
                    return "'";

                if (x == Symbol.QUASIQUOTE)
                    return "`";

                if (x == Symbol.UNQUOTE)
                    return ",";

                if (x == Symbol.UNQUOTESPLICE)
                    return ",@";
            }
            return x.Name;
        }
        static string InspectSchemeObjectIntern(Pair cons, InspectOptions options = InspectOptions.Default, bool encloseList = true)
        {
            StringBuilder sb = new StringBuilder();

            if (encloseList)
                sb.Append("(");

            Pair curent = cons;
            int consLen = 0;
            while (curent != null && ++consLen < Inspector.MAX_CONS_PRINT_LEN)
            {
                if (IsSpecialForm(curent.Car))
                    sb.Append(InspectSpecialReaderForm(curent.Car as Symbol, options));
                else
                    sb.Append(Inspector.Inspect(curent.Car, options));

                if (curent.Cdr == null)
                {
                    curent = null;
                    break;
                }
                if (curent.Cdr is Pair)
                {
                    sb.Append(" ");
                }
                else if (curent.Cdr != null)
                {
                    sb.Append(" . ");
                    sb.Append(Inspector.Inspect(curent.Cdr, options));
                    curent = null;
                    break;
                }
                curent = curent.Cdr as Pair;
            }

            if (curent != null)
                sb.Append(" ... ");

            if (encloseList)
                sb.Append(")");

            return sb.ToString();
        }
        static string InspectSchemeObjectIntern(SVector vector, InspectOptions options = InspectOptions.Default)
        {
            StringBuilder sb = new StringBuilder();
            bool appendSpace = false;
            sb.Append("#(");
            foreach (SObject v in vector.Value)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(Inspect(v, options));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }


        #region Inspect Mono Classes

        static string InspectMonoObject(object x, InspectOptions options = InspectOptions.Default)
        {
            if (x is string)
                return string.Format(CultureInfo.CurrentCulture, "\"{0}\"", x);

            if (x is char)
                return string.Format(CultureInfo.CurrentCulture, "#\\{0}", x);

            if (x is Array)
                return InspectArray((Array)x, options);

            if (Type.GetTypeCode(x.GetType()) == TypeCode.Object)
                return string.Format(CultureInfo.CurrentCulture, "#<{0}>", x.ToString().Trim());

            return x.ToString().Trim();

        }
        static string InspectArray(Array arr, InspectOptions options = InspectOptions.Default)
        {
            StringBuilder sb = new StringBuilder();

            // For large arrays, don't try to print the elements
            if (arr.Length > MAX_ARRAY_PRINT_LEN)
            {
                sb.Append("#<Array[");
                for (int ix = 0; ix < arr.Rank; ++ix)
                {
                    if (ix > 0)
                        sb.Append(" x ");
                    sb.Append(arr.GetUpperBound(ix) - arr.GetLowerBound(ix) + 1);
                }
                sb.Append("] ");
            }

            int[] ind = new int[arr.Rank];
            int[] lb = new int[arr.Rank];
            int[] ub = new int[arr.Rank];

            if (arr.Length <= MAX_ARRAY_PRINT_LEN)
                sb.Append(String.Format(CultureInfo.CurrentCulture,
                                        arr.Rank > 1 ? "#{0}a" : "#", arr.Rank));

            for (int ix = 0; ix < arr.Rank; ++ix)
            {
                ind[ix] = lb[ix] = arr.GetLowerBound(ix);
                ub[ix] = arr.GetUpperBound(ix);
                sb.Append("(");
            }

            int printedElts = 0;
            do
            {
                try
                {
                    sb.Append(Inspect(arr.GetValue(ind)));
                    ++printedElts;
                }
                catch (IndexOutOfRangeException)
                {
                    // ignore - we just don't print out 0-length arrays
                }
            }
            while (IncrementIndex(arr.Rank - 1, ind, lb, ub, sb) &&
                    printedElts < MAX_ARRAY_PRINT_LEN);

            if (arr.Length > MAX_ARRAY_PRINT_LEN)
            {
                sb.Append(" ... ");
                for (int ix = 0; ix < arr.Rank; ++ix)
                    sb.Append(')');
                sb.Append(">");
            }

            return sb.ToString();
        }

        internal static bool IncrementIndex(int ix, int[] ind, int[] lb, int[] ub, StringBuilder sb)
        {
            bool retval = false;
            if (ix >= 0)
            {
                if (ind[ix] < ub[ix])
                {
                    ++ind[ix];
                    sb.Append(" ");
                    retval = true;
                }
                else
                {
                    sb.Append(")");
                    ind[ix] = lb[ix];
                    retval = IncrementIndex(ix - 1, ind, lb, ub, sb);
                    if (retval)
                        sb.Append("(");
                }
            }

            return retval;
        }
        #endregion
    }
}
