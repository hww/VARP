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

using System;
using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using Tokenizing;
    using Data;
    using Exception;
    using DataStructures;

    /// <summary>
    /// Class that reads symbols from a Tokenizer and turns them into an object
    /// </summary>
    public sealed class Parser : ValueType
    {

        /// <summary>
        /// Parses a scheme expression in the default manner
        /// </summary>
        /// <returns>A scheme object</returns>
        /// <remarks>It is an error to pass scheme to this method with 'extraneous' tokens, such as trailing closing brackets</remarks>
        public static Syntax Parse(string scheme, string filepath)
        {
            var reader = new Tokenizer(new System.IO.StringReader(scheme), filepath);

            var res = Parse(reader);
            var token = reader.ReadToken();
            if (token != null) throw SchemeError.SyntaxError("parser", "found extra tokens after the end of a scheme expression", token);

            return res;
        }

        /// <summary>
        /// Turns the contents of a Tokenizer into an object
        /// </summary>
        /// <remarks>
        /// Recursive-descent via ParseToken. There may be tokens left in the reader.
        /// </remarks>
        public static Syntax Parse(Tokenizer reader)
        {
            var firstToken = reader.ReadToken();
            if (firstToken == null) return null;
            return ParseToken(firstToken, reader);
        }

        /// <summary>
        /// Turns thisToken into an object, using moreTokens to get further tokens if required
        /// </summary>
        /// <returns></returns>
        protected static Syntax ParseToken(Token thisToken, Tokenizer moreTokens)
        {
            if (thisToken == null)
                throw SchemeError.SyntaxError("parser", "unexpectedly reached the end of the input", moreTokens.LastToken);

            switch (thisToken.Type)
            {
                //case TokenType.Dot:
                //    return thisToken; //TODO maybe exception or symbol .

                case TokenType.Character:
                    return new Syntax(new Value(thisToken.GetCharacter()), thisToken);

                case TokenType.Boolean:
                    return new Syntax(new Value(thisToken.GetBool()), thisToken);

                case TokenType.String:
                    return new Syntax(new Value(thisToken.GetString()), thisToken);

                case TokenType.Symbol:
                    return new Syntax(thisToken.GetSymbol(), thisToken);

                case TokenType.Heximal:
                case TokenType.Integer:
                    return new Syntax(new Value(thisToken.GetInteger()), thisToken);

                case TokenType.Floating:
                    return new Syntax(new Value(thisToken.GetFloat()), thisToken);

                case TokenType.OpenBracket:
                    return ParseList(thisToken, moreTokens);

                case TokenType.OpenVector:
                    return ParseVector(thisToken, moreTokens);

                case TokenType.Quote:
                case TokenType.Unquote:
                case TokenType.UnquoteSplicing:
                case TokenType.QuasiQuote:
                    return ParseQuoted(thisToken, moreTokens);

                case TokenType.BadNumber:
                    throw SchemeError.SyntaxError("parser", "looks like it should be a number, but it contains a syntax error", thisToken);

                default:
                    // Unknown token type
                    throw SchemeError.SyntaxError("parser", "the element is being used in a context where it is not understood", thisToken);
            }
        }

        public static Syntax ParseQuoted(Token thisToken, Tokenizer moreTokens)
        {
            Symbol quote = null;

            // First symbol is quote, unquote, quasiquote depending on what the token was
            switch (thisToken.Type)
            {
                case TokenType.Quote: quote = Symbol.QUOTE; break;
                case TokenType.Unquote: quote = Symbol.UNQUOTE; break;
                case TokenType.QuasiQuote: quote = Symbol.QUASIQUOTE; break;
                case TokenType.UnquoteSplicing: quote = Symbol.UNQUOTESPLICE; break;
            }
            var quote_stx = new Syntax(quote, thisToken);
            var nextToken = moreTokens.ReadToken();
            var quoted = ParseToken(nextToken, moreTokens);
            var list = new LinkedList<Syntax>();
            list.AddLast(quote_stx);
            list.AddLast(quoted);
            return new Syntax(list, thisToken);
        }

        private static Syntax ParseDot(Token thisToken, Tokenizer moreTokens)
        {
            return null;
        }

        private static Syntax ParseList(Token thisToken, Tokenizer moreTokens)
        {
            // Is a list/vector
            var listContents = new List<Syntax>();
            Token dotToken = null;

            var nextToken = moreTokens.ReadToken();
            while (nextToken != null && nextToken.Type != TokenType.CloseBracket)
            {
                // Parse this token
                listContents.Add(ParseToken(nextToken, moreTokens));

                // Fetch the next token
                nextToken = moreTokens.ReadToken();
                if (nextToken == null)
                    throw SchemeError.SyntaxError("parser", "Improperly formed list.", dotToken);

                if (nextToken.Type == TokenType.Dot)
                {
                    if (dotToken != null || thisToken.Type != TokenType.OpenBracket)
                        throw SchemeError.SyntaxError("parser", "Improperly formed dotted list", nextToken);
                    dotToken = nextToken;
                    nextToken = moreTokens.ReadToken();
                    if (nextToken == null)
                        throw SchemeError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    if (nextToken.Type == TokenType.CloseBracket)
                        throw SchemeError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    listContents.Add(ParseToken(nextToken, moreTokens));
                    nextToken = moreTokens.ReadToken();
                    if (nextToken.Type != TokenType.CloseBracket)
                        throw SchemeError.SyntaxError("parser", "Improperly formed dotted list", dotToken);
                    break;
                }
            }

            if (nextToken == null)
            {
                // Missing ')'
                throw SchemeError.SyntaxError("parser", "missing close parenthesis", thisToken);
            }

            if (dotToken != null)
            {
                if (listContents.Count == 2)
                    return new Syntax(new ValuePair(listContents[0], listContents[1]), thisToken);
                else
                    throw SchemeError.SyntaxError("parser", "improper dot syntax", thisToken);
            }
            else
            {
                if (listContents.Count == 0)
                    return new Syntax(Value.Nil, thisToken);
                else
                    return new Syntax(ValueLinkedList.FromList<Syntax>(listContents), thisToken);
            }

        }

        private static Syntax ParseVector(Token thisToken, Tokenizer moreTokens)
        {
            var listContents = new List<object>();
            Token dotToken = null;

            var nextToken = moreTokens.ReadToken();
            while (nextToken != null && nextToken.Type != TokenType.CloseBracket)
            {
                // Parse this token
                listContents.Add(ParseToken(nextToken, moreTokens));

                // Fetch the next token
                nextToken = moreTokens.ReadToken();
                if (nextToken == null)
                    throw SchemeError.SyntaxError("parser", "Improperly formed list.", dotToken);

                //if (!improper && nextToken.Type == TokenType.Symbol && dotSymbol.Equals(nextToken.Value) && thisToken.Type == TokenType.OpenBracket)
                if (nextToken.Type == TokenType.Dot)
                    throw SchemeError.SyntaxError("parser", "Improperly formed dotted list", nextToken);
            }

            if (nextToken == null) // Missing ')'
                throw SchemeError.SyntaxError("parser", "Missing close parenthesis", thisToken);

            return new Syntax(ValueList.FromList(listContents), thisToken);
        }

        /// <summary>
        /// Works out how many brackets are missing for the expression given by the Tokenizer
        /// </summary>
        /// <param name="reader">The reader to read expressions from</param>
        /// <returns>The number of closing parenthesizes that are required to complete the expression (-1 if there are too many)</returns>
        public static int RemainingBrackets(Tokenizer reader)
        {
            var bracketCount = 0;
            Token thisToken;

            try
            {
                thisToken = reader.ReadToken();
            }
            catch (SchemeError)
            {
                thisToken = new Token(TokenType.BadSyntax, "", null);
            }
            catch (ArithmeticException)
            {
                thisToken = new Token(TokenType.BadNumber, "", null);
            }

            while (thisToken != null)
            {
                switch (thisToken.Type)
                {
                    case TokenType.OpenBracket:
                    case TokenType.OpenVector:
                        // If this begins a list or a vector, increase the bracket count
                        bracketCount++;
                        break;

                    case TokenType.CloseBracket:
                        // Close brackets indicate the end of a list or vector
                        bracketCount--;
                        break;
                }

                // Get the next token
                thisToken = reader.ReadToken();
            }

            // Set the count to -1 if there were too many brackets
            if (bracketCount < 0) bracketCount = -1;

            return bracketCount;
        }
    }
}

