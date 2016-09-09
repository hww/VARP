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

using UnityEngine;
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.Stx;
using VARP.Scheme.Data;
using VARP.Scheme.REPL;

[ExecuteInEditMode]
public class TokenizerTest : MonoBehaviour {

    Tokenizer lexer;

    [TextArea(20,100)]
    public string testString;
    [TextArea(10, 100)]
    public string tokensString;
    [TextArea(10, 100)]
    public string resultString;

    void Start()
    {
        OnValidate();
    }

    void OnValidate()
    {
        System.Text.StringBuilder sb;
        // ------------------------------------------------------------------
        // Just tokenized it
        // ------------------------------------------------------------------
        sb = new System.Text.StringBuilder();
        lexer = new Tokenizer(new StringReader(testString), "TokenizerTest");
        Token token = lexer.ReadToken();
        while (token != null)
        {
            sb.Append(Inspector.Inspect(token) + " ");
            token = lexer.ReadToken();
        }
        tokensString = sb.ToString();

        // ------------------------------------------------------------------
        // Parse scheme
        // ------------------------------------------------------------------
        lexer = new Tokenizer(new StringReader(testString), "TokenizerTest");
        sb = new System.Text.StringBuilder();
        do
        {
            SObject result = Parser.Parse(lexer);
            if (result == null) break;
            sb.AppendLine(result.Inspect());
        } while (lexer.LastToken != null);
        resultString = sb.ToString();
    }


}
