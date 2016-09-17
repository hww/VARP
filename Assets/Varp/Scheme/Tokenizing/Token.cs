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

using System.Globalization;
using UnityEngine;

namespace VARP.Scheme.Tokenizing
{
    using Data;
    using Exception;
    using Libs;

    public sealed class Token : ValueClass
    { 
        public TokenType Type;
        public string Value;
        public Location location;

        public Token()
        {
        }
        public Token(TokenType type, string value, Location location = null) 
        {
            this.Type = type;
            this.Value = value;
            this.location = location;
        }
        public Token(Token token) 
        {
            this.Type = token.Type;
            this.Value = token.Value;
            this.location = token.location;
        }



        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return Value.ToString(); }

        #endregion

        public bool GetBool()
        {
            Debug.Assert(Type == TokenType.Boolean);

            if (Value == "#t")
                return true;
            else if (Value == "#f")
                return false;
            else
                throw SchemeError.SyntaxError("get-bool", "improperly formed bool value", this);
        }

        public int GetInteger()
        {
            try
            {
                switch (Type)
                {
                    case TokenType.Integer:
                        return StringLibs.GetInteger(Value);

                    case TokenType.Heximal:
                        return StringLibs.GetHexadecimal(Value);

                    default:
                        throw SchemeError.SyntaxError("get-integer", "wrong token type", this);
                }
            }
            catch (System.Exception ex)
            {
                throw SchemeError.SyntaxError("get-integer", "improperly formed int value", this);
            }
        }

        public double GetFloat()
        {
            Debug.Assert(Type == TokenType.Floating);
            return StringLibs.GetFloat(Value);
        }
        public string GetString()
        {
            Debug.Assert(Type == TokenType.String);
            return Value;
        }

        public Symbol GetSymbol()
        {
            Debug.Assert(Type == TokenType.Symbol);
            return Symbol.Intern(Value);
        }

        public char GetCharacter()
        {
            Debug.Assert(Type == TokenType.Character);
            if (Value.Length == 3)
            {
                return System.Convert.ToChar(Value[2]);
            }
            else
            {
                char c = (char)0;
                if (CharClass.NameToCharacter(Value, out c))
                    return c;
                throw SchemeError.SyntaxError("get-character", "improperly formed char value", this);
            }
        }


    }


}