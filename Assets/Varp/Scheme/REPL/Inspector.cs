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
using System.Collections.Generic;

namespace VARP.Scheme.REPL
{
    using DataStructures;
    using Data;
    using Stx;
    using Tokenizing;

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
                return "null";

            if (x is Value)
                return InspectInternal((Value)x, options);

            if (x is ValueClass)
                return InspectInternal(x as ValueClass, options);

            if (x is Inspectable)
                return (x as Inspectable).Inspect();

            return InspectMonoObject(x, options);
        }

        private static string InspectInternal(Value x, InspectOptions options = InspectOptions.Default)
        {
            if (x.IsNil || x.IsBool || x.IsNumber)
                return x.ToString();
            return Inspect(x.RefVal, options);
        }

        private static string InspectInternal(ValueClass x, InspectOptions options = InspectOptions.Default)
        {
            if (x is Location)
                return InspectInternal(x as Location, options);
            if (x is Token)
                return InspectInternal(x as Token, options);
            if (x is ValuePair)
                return InspectInternal(x as ValuePair, options);
            if (x is Syntax)
                return InspectInternal(x as Syntax, options);
            if (x is AST)
                return InspectInternal(x as AST, options);
            if (x is AstBinding)
                return InspectInternal(x as AstBinding, options);
            if (x is AstEnvironment)
                return InspectInternal(x as AstEnvironment, options);
            // all another just convert to string
            return x.ToString();
        }

        private static string InspectInternal(Location x, InspectOptions options = InspectOptions.Default)
        {
            return string.Format("#<location:{0}:{1} {2}>", x.LineNumber, x.ColNumber, x.File);
        }

        private static string InspectInternal(Token x, InspectOptions options = InspectOptions.Default)
        {
            var loc = x.location;
            if (loc == null)
                return string.Format("#<token \"{0}\">", x.ToString());
            else
                return string.Format("#<token:{0}:{1} \"{2}\">", loc.LineNumber, loc.ColNumber, x.ToString());
        }

        private static string InspectInternal(Syntax x, InspectOptions options = InspectOptions.Default)
        {
            var loc = x.Location;
            if (loc == null)
                return string.Format("#<syntax {0}>", x.ToString());
            else
                return string.Format("#<syntax:{0}:{1} {2}>", loc.LineNumber, loc.ColNumber, Inspect(x.GetDatum(), options));
        }

        private static string InspectInternal(AstBinding bind, InspectOptions options = InspectOptions.Default)
        {
            var prefix = bind.IsPrimitive ? "#Prim" : string.Empty; 
                if (bind.IsGlobal)
                    return string.Format("{0} {1}", prefix, bind.Identifier.Name);
                else
                    return string.Format("[{0}] {1} {2}>", bind.VarIdx, prefix, bind.Identifier.Name);
        }

        private static string InspectInternal(AstEnvironment env, InspectOptions options = InspectOptions.Default)
        {
            var tabs = env.GetEnvironmentIndex();
            var tabstr = new string(' ', tabs * 4);
            var sb = new StringBuilder();
            sb.AppendLine(tabstr + "Lexical Environment");
            foreach (var b in env)
            {
                sb.AppendLine(tabstr + InspectInternal(b, options));
            }
            return sb.ToString();
        }

        private static string InspectInternal(ValuePair x, InspectOptions options = InspectOptions.Default)
        {
            return string.Format("({0} . {1})", Inspect(x.Item1, options), Inspect(x.Item2));
        }

        private static string InspectInternal(AST x, InspectOptions options = InspectOptions.Default)
        {
            return x.Inspect();
        }


        #region Inspect Mono Classes

        private static string InspectMonoObject(object x, InspectOptions options = InspectOptions.Default)
        {
            if (x is string)
                return string.Format(CultureInfo.CurrentCulture, "\"{0}\"", x);

            if (x is char)
                return string.Format(CultureInfo.CurrentCulture, "#\\{0}", x);

            if (x is LinkedList<Value>)
                return InspectInternal(x as LinkedList<Value>);

            if (x is List<Value>)
                return InspectInternal(x as List<Value>);

            if (x is Dictionary<object, Value>)
                return InspectInternal(x as Dictionary<object, Value>);

            if (x is Array)
                return InspectArray((Array)x, options);

            if (Type.GetTypeCode(x.GetType()) == TypeCode.Object)
                return string.Format(CultureInfo.CurrentCulture, "#<{0}>", x.ToString().Trim());

            return x.ToString().Trim();

        }

        private static string InspectArray(Array arr, InspectOptions options = InspectOptions.Default)
        {
            var sb = new StringBuilder();

            // For large arrays, don't try to print the elements
            if (arr.Length > MAX_ARRAY_PRINT_LEN)
            {
                sb.Append("#<Array[");
                for (var ix = 0; ix < arr.Rank; ++ix)
                {
                    if (ix > 0)
                        sb.Append(" x ");
                    sb.Append(arr.GetUpperBound(ix) - arr.GetLowerBound(ix) + 1);
                }
                sb.Append("] ");
            }

            var ind = new int[arr.Rank];
            var lb = new int[arr.Rank];
            var ub = new int[arr.Rank];

            if (arr.Length <= MAX_ARRAY_PRINT_LEN)
                sb.Append(String.Format(CultureInfo.CurrentCulture,
                                        arr.Rank > 1 ? "#{0}a" : "#", arr.Rank));

            for (var ix = 0; ix < arr.Rank; ++ix)
            {
                ind[ix] = lb[ix] = arr.GetLowerBound(ix);
                ub[ix] = arr.GetUpperBound(ix);
                sb.Append("(");
            }

            var printedElts = 0;
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
                for (var ix = 0; ix < arr.Rank; ++ix)
                    sb.Append(')');
                sb.Append(">");
            }

            return sb.ToString();
        }

        internal static bool IncrementIndex(int ix, int[] ind, int[] lb, int[] ub, StringBuilder sb)
        {
            var retval = false;
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

        private static string InspectInternal(LinkedList<Value> list, InspectOptions options = InspectOptions.Default, bool encloseList = true)
        {
            var sb = new StringBuilder();

            if (encloseList)
                sb.Append("(");

            var curent = list.First;
            var consLen = 0;
            while (curent != null && ++consLen < MAX_CONS_PRINT_LEN)
            {
                var sym = curent.Value.AsSymbol();
                if (sym != null && sym.IsSpecialForm)
                {
                    if (options == InspectOptions.PrettyPrint)
                        sb.Append(sym.ToSpecialFormString());
                    else
                        sb.Append(sym.ToString());
                }
                else
                    sb.Append(Inspect(curent.Value, options));

                curent = curent.Next;
                if (curent != null) sb.Append(" ");
            }

            if (curent != null)
                sb.Append(" ... ");

            if (encloseList)
                sb.Append(")");

            return sb.ToString();
        }

        private static string InspectInternal(List<Value> list, InspectOptions options = InspectOptions.Default)
        {
            var sb = new StringBuilder();
            var appendSpace = false;
            sb.Append("#(");
            foreach (var v in list)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(Inspect(v, options));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }

        private static string InspectInternal(Dictionary<object, Value> table, InspectOptions options = InspectOptions.Default)
        {
            var sb = new StringBuilder();
            var appendSpace = false;
            sb.Append("#hash(");
            foreach (var v in table)
            {
                if (appendSpace) sb.Append(" ");
                sb.Append(string.Format("#<pair {0} {1}>", Inspect(v.Key, options), Inspect(v.Value, options)));
                appendSpace |= true;
            }
            sb.Append(")");
            return sb.ToString();
        }
        #endregion
    }
}
