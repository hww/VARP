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
using System.IO;
using VARP.Scheme.Tokenizing;
using VARP.Scheme.Stx;
using VARP.Scheme.Data;
using VARP.Scheme.REPL;
using VARP.Scheme.Exception;
using VARP.Scheme.Codegen;
using VARP.Scheme.VM;
using Environment = VARP.Scheme.VM.Environment;

[ExecuteInEditMode]
public class Evaluator : MonoBehaviour
{
    private Tokenizer lexer;
    [TextArea(5, 100)]
    public string testString;
    public bool detailedSyntaxTree;
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

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        var sbsyntax = new System.Text.StringBuilder();
        var sbast = new System.Text.StringBuilder();
        var sbcode = new System.Text.StringBuilder();
        var sbeval = new System.Text.StringBuilder();

        try
        {
            // ------------------------------------------------------------------
            // Parse scheme
            // ------------------------------------------------------------------

            lexer = new Tokenizer(new StringReader(testString), "TokenizerTest");

            do
            {
                var syntax = Parser.Parse(lexer);
                if (syntax == null) break;
                if (detailedSyntaxTree)
                    sbsyntax.AppendLine(Inspector.Inspect(syntax, InspectOptions.PrettyPrint));
                else
                    sbsyntax.AppendLine(Inspector.Inspect(syntax));

                var ast = AstBuilder.Expand(syntax, SystemEnvironment.Top);
                sbast.AppendLine(ast.Inspect());

                var temp = CodeGenerator.GenerateCode(ast);
                sbcode.AppendLine(temp.Inspect());

                var vm = new VarpVM();
                var vmres = vm.RunTemplate(temp, SystemEnvironment.Top);
                if (vmres.RefVal is Frame)
                    vmres = vm.RunClosure(vmres.RefVal as Frame);
                sbeval.Append(Inspector.Inspect(vmres));

            } while (lexer.LastToken != null);

            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            InspectEnvironment();

        }
        catch (SchemeError ex)
        {
            sbeval.Append(ex.Message);

            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            InspectEnvironment();
        }
        catch (System.Exception ex)
        {
            syntaxString = sbsyntax.ToString();
            astString = sbast.ToString();
            codeString = sbcode.ToString();
            evalString = sbeval.ToString();
            InspectEnvironment();

            throw ex;
        }


    }

    private void InspectEnvironment()
    {
        envString = SystemEnvironment.Top.Inspect();
    }
}