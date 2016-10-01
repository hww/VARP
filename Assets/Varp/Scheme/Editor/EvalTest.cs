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

using System.CodeDom.Compiler;

namespace VARP.Scheme.Test
{
    using Tokenizing;
    using Stx;
    using REPL;
    using VM;
    using Data;
    using Codegen;

    public class EvalTest
    {

        [Test]
        public void Literals()
        {
            Evaluate("()", "nil");
            Evaluate("nil", "nil");
            Evaluate("0", "0");
            Evaluate("1234567890", "1234567890");
            Evaluate("#f", "#f");
            Evaluate("#t", "#t");
            Evaluate("\"hello world\"", "\"hello world\"");
            Evaluate(":key", ":key"); //< recognized as keyword
            Evaluate("#\\A", "#\\A");
        }

        [Test]
        public void Math()
        {
            Evaluate("(+ 6 4)", "10");
            Evaluate("(- 6 4)", "2");
            Evaluate("(* 6 4)", "24");
            Evaluate("(/ 6 4)", "1");
            Evaluate("(% 6 4)", "2");
            Evaluate("(neg 6)", "-6");
            Evaluate("(pow 6 2)", "36");
        }

        [Test]
        public void Logical()
        {
            Evaluate("(not #t)", "#f");
            Evaluate("(not #f)", "#t");

            Evaluate("(and #t #t #t)", "#t");
            Evaluate("(and #f #t #t)", "#f");
            Evaluate("(and #t #f #t)", "#f");
            Evaluate("(and #t #t #f)", "#f");
            Evaluate("(and #f #f #f)", "#f");

            Evaluate("(or #f #f #f)", "#f");
            Evaluate("(or #t #f #f)", "#t");
            Evaluate("(or #f #t #f)", "#t");
            Evaluate("(or #f #f #t)", "#t");
            Evaluate("(or #t #t #t)", "#t");
        }

        [Test]
        public void Concat()
        {
            Evaluate("(concat 1 2 3 4)", "\"1234\"");
            Evaluate("(concat \"hello\" \"world\")", "\"helloworld\"");
        }

        [Test]
        public void Lambda()
        {
            Evaluate("(lambda ())", "#<VARP.Scheme.VM.Frame>");
            Evaluate("((lambda ()))", "nil");
            Evaluate("((lambda () 999))", "999");
            Evaluate("((lambda (x) x) 999)", "999");
            Evaluate("((lambda (x y) (+ x y)) 1 2)", "3");
        }

        [Test]
        public void LmabdaOptionalArgs()
        {
            Evaluate("((lambda (x &optional y (z 9)) x) 5)", "5");
            Evaluate("((lambda (x &optional y (z 9)) y) 5 6)", "6");
            Evaluate("((lambda (x &optional y (z 9)) y) 5)", "#f");
            Evaluate("((lambda (x &optional y (z 8)) z) 5 6)", "8");
            Evaluate("((lambda (x &optional y (z 8)) z) 5 6 7)", "7");
        }

        void Evaluate (string source, string expectedResult)
        {
            

            try
            {
                Tokenizer lexer = new Tokenizer(new StringReader(source), "EvalTest.cs/sample code");

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                bool addSpace = false;
                do
                {
                    Syntax result = Parser.Parse(lexer);
                    if (result == null) break;

                    AST ast = AstBuilder.Expand(result);
                    Template temp = CodeGenerator.GenerateCode(ast);
                    VarpVM vm = new VarpVM();
                    Value vmres = vm.RunTemplate(temp);

                    if (addSpace) sb.Append(" "); else addSpace = true;
                    sb.Append(Inspector.Inspect(vmres));
                    
                } while (lexer.LastToken != null);

                string sresult = sb.ToString();

                Debug.Assert(sresult == expectedResult, FoundAndExpected(source, sresult, expectedResult));
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("{0}\n{1}\n{2}", source, ex.Message, ex.StackTrace));
            }
        }

        string FoundAndExpected(string source, string found, string expected)
        {
            return string.Format(" SOURCE: {0}\n EXPECTED:\n{1}\n FOUND:\n{2}", source, expected, found);
        }
    }
}