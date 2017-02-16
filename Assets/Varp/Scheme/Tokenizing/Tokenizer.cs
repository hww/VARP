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
using System.IO;
using System.Linq;

namespace VARP.Scheme.Tokenizing
{
    using Exception;
    using Utils;
    public sealed class Tokenizer
    {
        public TokenReader reader;
        protected BetterStringBuilder builder;

        protected int Character;
        protected Token lastToken;
        /// <summary>
        /// Tokenize stream
        /// </summary>
        /// <param name="input">Text stream</param>
        /// <param name="filePath">File path used to tokens location only</param>
        /// <returns></returns>
        /// <usage>
        ///     Tokenizer tokenizer = new Tokenizer(new TextReader(), "FILE PATH");
        ///     List<Token> tokenList = new List<Token>();
        ///     Token token;
        ///     while (GetNextToken(ref token))
        ///         tokenList.Add(token);
        /// </usage>
        public Tokenizer(TextReader input, string filePath)
        {
            builder = new BetterStringBuilder(1024);
            reader = new TokenReader(input, filePath);
            Character = -1;
            builder.Clear();

            // Get next character
            NextChar();
        }

        /// <summary>
        /// Tokenize to the tokens list
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public void TokenizeToList(List<Token> tokenList)
        {
            var token = GetNextToken();
            while (token != null)
            {
                tokenList.Add(token);
                token = GetNextToken();
            }
        }

        private char cCharacter;
        private void NextChar() { Character = reader.Read(); cCharacter = (char)Character; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Next token or null</returns>
        private Token GetNextToken()
        {
            // Skip whitespace
            while (IsWhitespace(Character))
                NextChar();

            // Skip comments and whitespace again, since we can have whitespace after comments etc.
            while (Character == ';')
            {
                while (Character>=0 && !IsNewline(Character)) NextChar();
                while (IsWhitespace(Character)) NextChar();
            }
            lastToken = new Token();
            DefineTokenLocation();

            // get the next token
            if (Character >= 0)
            {
                if (IsNumerical(Character))
                    return NumberLiteral();

                switch (Character)
                {
                    case '#':
                        return VectorOrBooleanOrChar();
                    case '"':
                        return StringLiteral();
                    case '-':
                    case '+':

                        // only a number literal if the starting char is numerical, or if the starting char is a - or +
                        // AND the next char is part of a number.
                        var nextc = reader.Peek();
                        if (IsNumerical(nextc) || nextc == '.')
                            return NumberLiteral();
                        else
                            return Symbol();
                    case '\'':
                    case '`':
                    case ',':
                        return QuoteSugar();
                    case '(':
                    case '[':
                        NextChar();
                        return DefineToken(TokenType.OpenBracket, "(");
                    case ')':
                    case ']':
                        NextChar();
                        return DefineToken(TokenType.CloseBracket, ")");
                    case '.':
                        NextChar();
                        return DefineToken(TokenType.Dot, ".");
                    default:
                        return Symbol();
                }
            }
            else
            {
                // no more characters, return an empty KVP
                return null;
            }
        }

        // retrieve a string literal token and advance our position
        private Token StringLiteral()
        {
            builder.Clear();
            NextChar();
            var type = TokenType.String;

            var matchingQuotes = false;
            while (Character >= 0)
            {
                // if we get an escape, increment the position some more and map the escaped character to what it should be
                if (Character == '\\')
                {
                    NextChar();
                    builder.Append(MapEscaped(Character));
                    NextChar();
                    continue;
                }

                // unescaped quote? We're done with this string.
                if (Character == '"')
                {
                    NextChar();
                    matchingQuotes = true;
                    break;
                }

                builder.Append((char)Character);
                NextChar();
            }

            // we didn't get opening and closing quotes :(
            if (!matchingQuotes)
            {
                throw SchemeError.SyntaxError("tokenizer", "unmatched quotes in string literal", lastToken);
            }

            return DefineToken(type, builder.ToString());
        }

        // retrieve a symbol token and advance our position
        private Token Symbol()
        {
            var type = TokenType.Symbol;
            builder.Clear();
            // as long as we don't hit whitespace or something that a symbol shouldn't have, assume it's a symbol
            while (Character >= 0 && !IsWhitespace(Character) && IsSymbolPart(Character))
            {
                builder.Append((char)Character);
                NextChar();
            }

            return DefineToken(type, builder.ToString());
        }

        // retrieve a quoted sugar token (' ` , ,@) and advance our position
        private Token QuoteSugar()
        {
            TokenType type;
            builder.Clear();

            switch (Character)
            {
                case '\'':
                    type = TokenType.Quote;
                    break;
                case '`':
                    type = TokenType.QuasiQuote;
                    break;
                case ',':
                    type = TokenType.Unquote;
                    break;
                default:
                    throw SchemeError.SyntaxError("tokenizer", "unexpected character", lastToken);

            }

            builder.Append((char)Character);
            NextChar();
            if (Character == '@')
            {
                type = TokenType.UnquoteSplicing;
                NextChar();
                builder.Append((char)Character);
            }

            return DefineToken(type, builder.ToString());
        }

        // retrieve a number literal token (int or decimal) and advance our position
        private Token NumberLiteral()
        {
            var type = TokenType.Integer;
            builder.Clear();

            while (Character >= 0 && IsNumericalPart(Character))
            {
                // if we get a decimal we're no longer working with an integer 
                if (Character == '.')
                {
                    if (type == TokenType.Floating)
                        throw SchemeError.SyntaxError("tokenizer", "error in numerical literal", lastToken);

                    type = TokenType.Floating;
                }
                builder.Append((char)Character); 
                NextChar();
            }

            return DefineToken(type, builder.ToString());
        }

        // retrieve a vector literal marker, boolean token, or character literal token and advance our position
        private Token VectorOrBooleanOrChar()
        {
            builder.Clear();
            var boolLiterals = new List<char> { 'F', 'f', 't', 'T' };
            var nextc = reader.Peek();
            if (boolLiterals.Contains((char)nextc))
            {
                // boolean literal!
                builder.Append((char)Character);
                NextChar(); // skip #
                if (Character>=0) {
                    builder.Append((char)Character);
                    NextChar(); // skip F,f,t,T
                }
                return DefineToken(TokenType.Boolean, builder.ToString());
            }
            else if (nextc == '(')
            {
                // vector literal!
                NextChar(); // skip #
                NextChar(); // skip (
                return DefineToken(TokenType.OpenVector, "(");
            }
            else if (nextc == '\\')
            {
                // char literal!
                while (Character >= 0 && !IsWhitespace(Character))
                {
                    if (builder.Size < 3)
                    {
                        builder.Append((char)Character);
                        NextChar();
                    }
                    else
                    {
                        if (!IsEOF(Character) && IsSymbolPart(Character))
                        {
                            builder.Append((char)Character);
                            NextChar();
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return DefineToken(TokenType.Character, builder.ToString());
            }
            else if (nextc == 'x')
            {
                builder.Append((char)Character);
                NextChar(); // skip #
                builder.Append((char)Character);
                NextChar(); // skip x
                // char literal!
                while (Character >= 0 && IsHeximalPart(Character))
                {
                    builder.Append((char)Character);
                    NextChar();
                }
                return DefineToken(TokenType.Heximal, builder.ToString());
            }
            //else if (nextc == '!')
            //{
            //    builder.Append((char)Character);
            //    NextChar(); // skip #
            //    builder.Append((char)Character);
            //    NextChar(); // skip !
            //    // char literal!
            //    while (Character >= 0 && IsSymbolPart(Character))
            //    {
            //        builder.Append((char)Character);
            //        NextChar();
            //    }
            //    return DefineToken(TokenType.Symbol, builder.ToString());
            //}
            else
            {
                throw SchemeError.SyntaxError("tokenizer", "inside '#' but no matching characters to construct a token", lastToken);
            }
        }

        #region PREDICATES

        private bool IsEOF(int inChar)
        {
            return inChar < 0;
        }

        private bool IsNumerical(int inChar)
        {
            return Char.IsNumber((char)inChar);
        }

        private bool IsNumericalPart(int inChar)
        {
            char[] signs = { '-', '+' };
            return inChar == '.' || IsNumerical(inChar) || signs.Contains((char)inChar);
        }

        private bool IsHeximalPart(int inChar)
        {
            char[] signs = {  };
            return IsNumerical(inChar) || (inChar >= 'A' && inChar <= 'F') || signs.Contains((char)inChar);
        }

        private bool IsSymbolPart(int inChar)
        {
            char[] notAllowed = { '(', ')', '[', ']', '"', '\'', ',', '`' };
            return !notAllowed.Contains((char)inChar);
        }

        private bool IsNewline(int inChar)
        {
            return inChar == '\r' || inChar == '\n';
        }

        private bool IsWhitespace(int inChar)
        {
            return inChar >= 0 && inChar <= ' '; // Any character less that space is space
        }

        private char MapEscaped(int escaped)
        {
            switch (escaped)
            {
                case 't':
                    return '\t';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
            }

            return (char)escaped;
        }

        #endregion

        private Token DefineToken(TokenType type, String value)
        {
            lastToken.Type = type;
            lastToken.Value = value;
            return lastToken;
        }

        private void DefineTokenLocation()
        {
            lastToken.location = new Location();
            lastToken.location.CharNumber = reader.CharNumber;
            lastToken.location.LineNumber = reader.LineNumber;
            lastToken.location.ColNumber = reader.ColNumber;
            lastToken.location.File = reader.FilePath;
        }

        #region ITokenizer

        /// <summary>
        /// Tokenize stream
        /// </summary>
        /// <param name="input">Text stream</param>
        /// <param name="filePath">File path used to tokens location only</param>
        /// <returns></returns>
        /// <usage>
        ///     Tokenizer tokenizer = new Tokenizer();
        ///     tokenizer.Tokenize(new TextReader(), "FILE PATH");  
        ///     List<Token> tokenList = new List<Token>();
        ///     Token token = GetNextToken();
        ///     while (token != null)
        ///     {
        ///          tokenList.Add(token);
        ///          token = GetNextToken();
        ///     }
        /// </usage>
        public Token ReadToken() { return GetNextToken(); }
        public Token LastToken { get { return lastToken; } }
        public int CharNumber { get { return reader.CharNumber; } }
        public int LineNumber { get { return reader.LineNumber; } }
        public int ColNumber { get { return reader.ColNumber; } }
        public string FilePath { get { return reader.FilePath; } }

        #endregion
    }
}