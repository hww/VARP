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
using VARP.Scheme.Exception;
using VARP.Scheme.Codegen;
using VARP.Scheme.VM;

[ExecuteInEditMode]
public class Evaluator : MonoBehaviour
{

    Tokenizer lexer;

    [TextArea(5, 100)]
    public string testString;
    [TextArea(5, 100)]
    public string syntaxString;
    [TextArea(10, 100)]
    public string astString;
    [TextArea(10, 100)]
    public string codeString;
    [TextArea(5, 100)]
    public string evalString;
    [TextArea(5, 100)]
    public string envString;

    void Start()
    {
        OnValidate();
    }

    void OnValidate()
    {
        System.Text.StringBuilder sbsyntax = new System.Text.StringBuilder();
        System.Text.StringBuilder sbast = new System.Text.StringBuilder();
        System.Text.StringBuilder sbcode = new System.Text.StringBuilder();
        System.Text.StringBuilder sbeval = new System.Text.StringBuilder();

        try
        {
            // ------------------------------------------------------------------
            // Parse scheme
            // ------------------------------------------------------------------

            lexer = new Tokenizer(new StringReader(testString), "TokenizerTest");

            do
            {
                Syntax syntax = Parser.Parse(lexer);
                if (syntax == null) break;
                sbsyntax.AppendLine(Inspector.Inspect(syntax));

                AST ast = AstBuilder.Expand(syntax);
                sbast.AppendLine(ast.Inspect());

                Template temp = CodeGenerator.GenerateCode(ast);
                sbcode.AppendLine(temp.Inspect());

                VarpVM vm = new VarpVM();
                Value vmres = vm.RunTemplate(temp);
                sbeval.Append(Inspector.Inspect(vmres));

            } while (lexer.LastToken != null);

            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            envString = AstBuilder.environment.Inspect();

        }
        catch (SchemeError ex)
        {
            sbeval.Append(ex.Message);

            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            envString = AstBuilder.environment.Inspect();
        }
        catch (System.Exception ex)
        {
            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            envString = AstBuilder.environment.Inspect();

            throw ex;
        }


    }
}