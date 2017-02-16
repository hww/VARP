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

using System.Collections.Generic;

namespace VARP.Scheme.Data
{
    /// <summary>
    /// Class representing a scheme symbol
    /// </summary>
    public sealed class Symbol : ValueClass
    {
        private string name;    //< symbol's name
        private bool keyword;   //< is this symbol the keyword

        /// <summary>
        /// Create new symbol
        /// Any symbol started with ':' character marked as 'keyword'
        /// The keywords are literals
        /// </summary>
        /// <param name="name">symbol name</param>
        private Symbol(string name)
        {
            this.name = name;
            keyword = name[0] == ':';
        }

        /// <summary>
        /// Return symbol's name
        /// </summary>
        public string Name { get { return name; } }

        #region ValueType Methods

        public override bool AsBool() { return true; }
        public override string ToString() { return Name; }
        public bool IsLiteral { get { return keyword; } }
        public bool IsIdentifier { get { return !keyword; } }

        public bool IsSpecialForm
        {
            get
            {
                return this == QUOTE ||
                        this == QUASIQUOTE ||
                        this == UNQUOTE ||
                        this == UNQUOTESPLICE;
            }
        }

        public string ToSpecialFormString()
        {
            if (this == QUOTE)
                return "'";

            if (this == QUASIQUOTE)
                return "`";

            if (this == UNQUOTE)
                return ",";

            if (this == UNQUOTESPLICE)
                return ",@";
            return Name;
        }

        #endregion

        #region Symbol Factory

        private static Dictionary<string, Symbol> internedSymbols = new Dictionary<string, Symbol>();
        public static Symbol Intern(string name)
        {
            Symbol value;
            if (internedSymbols.TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                value = new Symbol(name);
                internedSymbols[name] = value;
                return value;
            }
        }

        #endregion
        #region Commonly used symbols

        public static string SYSTEM = "system";
        public static Symbol QUOTE = Intern("quote");                        // '
        public static Symbol UNQUOTE = Intern("unquote");                    // ,
        public static Symbol QUASIQUOTE = Intern("quasiquote");              // `
        public static Symbol UNQUOTESPLICE = Intern("unquote-splicing");     // ,@
        public static Symbol DOT = Intern(".");
        public static Symbol LAMBDA = Intern("lambda");
        public static Symbol AREF = Intern("aref");
        public static Symbol ARRAY = Intern("array");
        public static Symbol CALL = Intern("call");
        public static Symbol CALLINST = Intern("call-instance");
        public static Symbol DEBUG = Intern("debug");
        public static Symbol EVAL = Intern("eval");
        public static Symbol FUNC = Intern("function");
        public static Symbol MACEXP1 = Intern("macroexpand-1");
        public static Symbol MD_ARRAY = Intern("md-array");
        public static Symbol NEW = Intern("new");
        public static Symbol NTH = Intern("nth");
        public static Symbol NULL = Intern("nil");
        public static Symbol BEGIN = Intern("begin");
        public static Symbol RETURN = Intern("return");
        public static Symbol SELF = Intern("self");
        public static Symbol SETQ = Intern("=");
        public static Symbol SET = Intern("set!");
        public static Symbol IF = Intern("if");
        public static Symbol ELSE = Intern("else");
        public static Symbol TRACE = Intern("trace");
        public static Symbol BODY = Intern("&body");
        public static Symbol KEY = Intern("&key");
        public static Symbol OPTIONAL = Intern("&optional");
        public static Symbol REST = Intern("&rest");
        public static Symbol ENVIRONMENT = Intern("*environment*");
        public static Symbol ERROR = Intern("*error*");
        public static Symbol INPUT = Intern("*input*");
        public static Symbol LAST_EXCPT = Intern("*last-exception*");
        public static Symbol MAX_RECUR_DEPTH = Intern("*max-recursion-depth*");
        public static Symbol OUTPUT = Intern("*output*");
        public static Symbol READTABLE = Intern("*readtable*");
        public static Symbol SETF_DISP = Intern("*setf-dispatch*");

        public static Symbol COND = Intern("cond");                          // do not required by spec, added for performance
        #endregion
    }
}
