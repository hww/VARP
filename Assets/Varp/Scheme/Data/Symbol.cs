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
    public sealed class Symbol : SObject, ISymbolic
    {
        private static Dictionary<string, Symbol> internedSymbols = new Dictionary<string, Symbol>();

        private string name;
        public string Name { get { return name; } }

        private Symbol(string name) { this.name = name; }

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

        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
        public override string AsString() { return Name; }
        public override bool IsLiteral { get { return name[0] == ':'; } }
        public override bool IsIdentifier { get { return name[0] != ':'; } }
        #endregion

        #region ISymbolic

        Scheme.Data.Symbol Scheme.Data.ISymbolic.Symbol { get { return this; } }
        public object HashValue { get { return GetHashCode(); } }
        //public GlobalEnvironment Location { get { return null; } }

        #endregion


        #region Commonly used symbols

        public static string SYSTEM = "system";
        public static Symbol QUOTE = Symbol.Intern("quote");                        // '
        public static Symbol UNQUOTE = Symbol.Intern("unquote");                    // ,
        public static Symbol QUASIQUOTE = Symbol.Intern("quasiquote");              // `
        public static Symbol UNQUOTESPLICE = Symbol.Intern("unquote-splicing");     // ,@
        public static Symbol DOT = Symbol.Intern(".");
        public static Symbol LAMBDA = Symbol.Intern("lambda");
        public static Symbol AREF = Symbol.Intern("aref");
        public static Symbol ARRAY = Symbol.Intern("array");
        public static Symbol CALL = Symbol.Intern("call");
        public static Symbol CALLINST = Symbol.Intern("call-instance");
        public static Symbol DEBUG = Symbol.Intern("debug");
        public static Symbol EVAL = Symbol.Intern("eval");
        public static Symbol FUNC = Symbol.Intern("function");
        public static Symbol MACEXP1 = Symbol.Intern("macroexpand-1");
        public static Symbol MD_ARRAY = Symbol.Intern("md-array");
        public static Symbol NEW = Symbol.Intern("new");
        public static Symbol NTH = Symbol.Intern("nth");
        public static Symbol NULL = Symbol.Intern("null");
        public static Symbol BEGIN = Symbol.Intern("begin");
        public static Symbol RETURN = Symbol.Intern("return");
        public static Symbol SELF = Symbol.Intern("self");
        public static Symbol SETQ = Symbol.Intern("=");
        public static Symbol SET = Symbol.Intern("set!");
        public static Symbol IF = Symbol.Intern("if");
        public static Symbol ELSE = Symbol.Intern("else");
        public static Symbol TRACE = Symbol.Intern("trace");
        public static Symbol BODY = Symbol.Intern("&body");
        public static Symbol KEY = Symbol.Intern("&key");
        public static Symbol OPTIONAL = Symbol.Intern("&optional");
        public static Symbol REST = Symbol.Intern("&rest");
        public static Symbol ENVIRONMENT = Symbol.Intern("*environment*");
        public static Symbol ERROR = Symbol.Intern("*error*");
        public static Symbol INPUT = Symbol.Intern("*input*");
        public static Symbol LAST_EXCPT = Symbol.Intern("*last-exception*");
        public static Symbol MAX_RECUR_DEPTH = Symbol.Intern("*max-recursion-depth*");
        public static Symbol OUTPUT = Symbol.Intern("*output*");
        public static Symbol READTABLE = Symbol.Intern("*readtable*");
        public static Symbol SETF_DISP = Symbol.Intern("*setf-dispatch*");

        public static Symbol COND = Symbol.Intern("cond");                          // do not required by spec, added for performance
        #endregion
    }
}
