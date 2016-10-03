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


namespace VARP.Scheme.Test
{
    using Tokenizing;
    using Stx;
    using REPL;

    public class AstTest
    {

        string[] tests = new string[]
        {
        // List
        "()","nil",
        "'()","(quote nil)",
        "(()())","(nil nil)", // Nested List
        // Numbers
        "1 1.0 #xFF","1 1.0 255",           
        // Strings    
        "\"foo\" \"bar\"","\"foo\" \"bar\"",  
        // Symbols
        "+ -","+ -",
        // Boolean
        "#t #f","#t #f",            
        // Characters
        "#\\A #\\space","#\\A #\\space",
        // Array
        "#(1 2)","#(1 2)",
        // Dot syntax
        "'(1 . 2)","(quote (1 . 2))",
        // Quotes
        "'(,() `() ,@())","(quote ((unquote nil) (quasiquote nil) (unquote-splicing nil)))",
        // Lambda
        "(lambda (x y) (+ x y) (- x y))","(lambda (x y) (+ x y) (- x y))",
        "(lambda (x y &optional (o 1) (o 2) &key (k 1) (k 2) &rest r))","(lambda (x y &optional (o 1) (o 2) &key (k 1) (k 2) &rest r))",
        // Let
        "(let ((x 1) (y 2)) 3 4)","(let ((x 1) (y 2)) 3 4)",
        // Conditions
        "(if 1 2)","(if 1 2)",
        "(if 1 2 3)","(if 1 2 3)",
        "(if (1) (2) (3))","(if (1) (2) (3))",
        "(cond (1 2) (3 4) (else 5))","(cond (1 2) (3 4) (else 5))",
        "(cond (1 2) (3 4))","(cond (1 2) (3 4))",
        // Application
        "(display 1 2)","(display 1 2)",
        // Variable reference
        "+", "+",
        // Primitives
        "(not 1)","(not 1)",
        "(display 1 2 2 3)","(display 1 2 2 3)",
        "(and 1 2 2 3)","(and 1 2 2 3)"
        };

        [Test]
        public void AstTestRun()
        {
            for (int i = 0; i < tests.Length; i += 2)
                Test(tests[i], tests[i + 1]);
        }

        void Test(string source, string expectedResult)
        {
            try
            {
                Tokenizer lexer = new Tokenizer(new StringReader(source), "AstTest.cs/sample code");

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                bool addSpace = false;
                do
                {
                    Syntax result = Parser.Parse(lexer);

                    if (result == null) break;

                    AST ast = AstBuilder.Expand(result);

                    if (addSpace) sb.Append(" "); else addSpace = true;
                    sb.Append(Inspector.Inspect(ast.GetDatum()));
                    
                } while (lexer.LastToken != null);
                string sresult = sb.ToString();
                Assert.AreEqual(sresult, expectedResult);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("{0}\n{1}\n{2}", source, ex.Message, ex.StackTrace));
            }
        }
    }
}