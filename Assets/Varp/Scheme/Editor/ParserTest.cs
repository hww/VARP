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

using UnityEngine;
using NUnit.Framework;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.Stx;
using VARP.Scheme.Data;
namespace VARP.Scheme.Test
{
    public class ParserTest
    {

        string[] tests = new string[]
        {
            // List
            "()","#<syntax:1:1 nil>",
            "'()","#<syntax:1:1 (quote nil)>",
            "(()())","#<syntax:1:1 (nil nil)>", // Nested List
            // Numbers
            "1 1.1 #xFF","#<syntax:1:1 1> #<syntax:1:3 1.1> #<syntax:1:7 255>",           
            // Strings    
            "\"foo\" \"bar\"","#<syntax:1:1 \"foo\"> #<syntax:1:7 \"bar\">",  
            // Symbols
            "foo bar","#<syntax:1:1 foo> #<syntax:1:5 bar>",          
            // Boolean
            "#t #f","#<syntax:1:1 #t> #<syntax:1:4 #f>",            
            // Characters
            "#\\A #\\space","#<syntax:1:1 #\\A> #<syntax:1:5 #\\space>",
            // Array
            "#(1 2)","#<syntax:1:1 #(1 2)>",
            // Dot syntax
            "(1 . 2)","#<syntax:1:1 (1 . 2)>",
            // Quotes
            "'(,() `() ,@())","#<syntax:1:1 (quote ((unquote nil) (quasiquote nil) (unquote-splicing nil)))>",
        };

        [Test]
        public void ParserTestRun()
        {
            for (int i = 0; i < tests.Length; i += 2)
                Test(tests[i], tests[i + 1]);
        }

        void Test(string source, string expectedResult)
        {
            //Prse

            try
            {
                Tokenizer lexer = new Tokenizer(new StringReader(source), "TokenizerTest");
                Parser parser = new Parser();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                bool assSpace = false;
                do
                {
                    ValueClass result = Parser.Parse(lexer);
                    if (result == null) break;
                    if (assSpace) sb.Append(" ");
                    sb.Append(result.Inspect());
                    assSpace = true;
                } while (lexer.LastToken != null);
                string sresult = sb.ToString();
                Debug.Assert(sresult == expectedResult, FoundAndExpected(sresult, expectedResult));
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("{0}\n{1}\n{2}", source, ex.Message, ex.StackTrace));
            }
        }

        string FoundAndExpected(string found, string expected)
        {
            return string.Format(" EXPECTED:\n{0}\n FOUND:\n{1}", expected, found);
        }
    }
}