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

namespace VARP.Scheme
{
    using VM;
    using Stx;
    using Data;
    using VARP.Scheme.Tokenizing;
    using System.IO;
    using VARP.Scheme.Codegen;

    public class Scheme
    {
        #region Evaluator

        public static Value EvaluateFile(string filePath, Environment environment)
        {
            var file = File.OpenText(filePath);

            var lexer = new Tokenizer(file, filePath);

            var result = Value.Nil;

            do
            {
                // parse code
                var syntax = Parser.Parse(lexer);
                if (syntax == null) break;

                // evaluate code
                result = Evaluate(syntax, environment);

            } while (lexer.LastToken != null);

            return result;
        }

        public static Value Evaluate(string expression, string filePath, Environment environment)
        {
            var lexer = new Tokenizer(new StringReader(expression), filePath);

            var result = Value.Nil;

            do
            {
                // parse code
                var syntax = Parser.Parse(lexer);
                if (syntax == null) break;
                
                // evaluate code
                result = Evaluate(syntax, environment);

            } while (lexer.LastToken != null);

            return result;
        }

        public static Value Evaluate(Value expression, string filePath, Environment environment)
        {
            if (expression.IsSyntax)
            {
                return Evaluate(expression.AsSyntax(), environment);
            }
            else
            {
                var location = null as Location;
                if (filePath != null)
                    location = new Location(0,0,0, filePath);
                var syntax = Syntax.GetSyntax(expression, location);
                return Evaluate(syntax.AsSyntax(), environment);
            }
        }

        public static Value Evaluate(Syntax syntax, Environment environment, Frame frame = null)
        {
            var result = Value.Nil;

            // create environment
            var lexical = null as Environment;

            if (frame != null)
                lexical = Environment.Create(environment, Symbol.NULL, frame);

            lexical = new Environment(environment, Symbol.Intern("LEXICAL"), true);

            // expand code
            var ast = AstBuilder.Expand(syntax, lexical);

            // generate the code
            var lambda = new CodeGenerator(lexical.Count);
            var regnum = lambda.GenerateTopLambda(lexical, ast);
            var template = lambda.GetTemplate();

            // evaluate code
            VarpVM vm = new VarpVM();
            result = vm.RunTemplate(template, environment, frame);

            if (result.RefVal is Frame)
                result = vm.RunClosure(result.RefVal as Frame);

            return result;
        }

        #endregion


        #region Compilter

        public static Value CompileFile(string filePath)
        {
            return Value.Nil;
        }

        public static Value Compile(string expression, string filePath, Environment environment = null)
        {
            var lexer = new Tokenizer(new StringReader(expression), filePath);

            var result = Value.Nil;

            do
            {
                // parse code
                var syntax = Parser.Parse(lexer);
                if (syntax == null) break;

                // evaluate code
                result = Evaluate(syntax, environment);

            } while (lexer.LastToken != null);

            return result;
        }

        public static Value Compile(Value expression, Environment environment = null)
        {
            return Value.Nil;
        }

        public static Template Compile(Syntax syntax, Environment environment = null)
        {
            // expand code
            AST ast = AstBuilder.Expand(syntax, environment);
            return CodeGenerator.GenerateCode(ast);
        }

        #endregion
    }
}
