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
using UnityEditor;
using NUnit.Framework;
using System.IO;
using VARP.Scheme.Tokenizing;

public class TokenizerTest {

    string[] tests = new string[]
    {
        // List
        "()","(:OpenBracket ):CloseBracket",        
        "(()())","(:OpenBracket (:OpenBracket ):CloseBracket (:OpenBracket ):CloseBracket ):CloseBracket", // Nested List
        // Numbers
        "1 1.0","1:Integer 1.0:Floating",           
        // Strings    
        "\"foo\" \"bar\"","foo:String bar:String",  
        // Symbols
        "foo bar","foo:Symbol bar:Symbol",          
        // Boolean
        "#t #f","#t:Boolean #f:Boolean",            
        // Characters
        "#\\A #\\space","#\\A:Character #\\space:Character", 
        "#(1 2)","(:OpenVector 1:Integer 2:Integer ):CloseBracket",
        // Dot syntax
        "(1 . 2)","(:OpenBracket 1:Integer .:Dot 2:Integer ):CloseBracket",
        // Quotes
        "'(,() `() ,@())","':Quote (:OpenBracket ,:Unquote (:OpenBracket ):CloseBracket `:QuasiQuote (:OpenBracket ):CloseBracket ,(:UnquoteSplicing (:OpenBracket ):CloseBracket ):CloseBracket",
    };

    [Test]
    public void TokenizerTestRun()
    {
        for (int i=0; i<tests.Length; i+=2)
            Test(tests[i], tests[i+1]);
    }

    void Test(string source, string expectedResult)
    {
        // Just tokenized it
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Tokenizer lexer = new Tokenizer(new StringReader(source), "TokenizerTest");
        Token token = lexer.ReadToken();
        bool separator = false;
        while (token != null)
        {
            if (separator) sb.Append(" ");
            sb.Append(token.Value + ":" + token.Type.ToString());
            token = lexer.ReadToken();
            separator = true;
        }
        string result = sb.ToString();
        Debug.Assert(result == expectedResult, FoundAndExpected(result, expectedResult));
    }

    string FoundAndExpected(string found, string expected)
    {
        return string.Format(" EXPECTED:\n{0}\n FOUND:\n{1}", expected, found);
    }
}
