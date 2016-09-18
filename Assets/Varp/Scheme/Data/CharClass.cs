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

namespace VARP.Scheme.Data
{

    /// <summary>
    /// Represents a single character string.
    /// </summary>
    /// <remarks>
    /// These strings may contain nulls and are not null-terminated.
    /// </remarks>
    [Serializable]
    public sealed class CharClass : NumberClass
    {
        public static CharClass Instance = new CharClass();
        public string ToString(double value) { return CharacterToName(Convert.ToChar(value)); }

        #region Named Characters

        static Dictionary<string, char> stringToCharMap;
        static string[] charToStringMap = new string[256];

        // TODO
        // Key Bucky bit prefix        Bucky bit
        // ---      ----------------    ---------
        // 
        // Meta     M- or Meta-                 1
        // Control  C- or Control-              2
        // Super    S- or Super-                4
        // Hyper    H- or Hyper-                8
        // Top      T- or Top-                 16
        // For example,
        // 
        // #\c-a                   ; Control-a
        // #\meta-b                ; Meta-b
        // #\c-s-m-h-a             ; Control-Meta-Super-Hyper-A

        static void DefineMapNamedLiteral(int character, string name)
        {
            stringToCharMap.Add(name, (char)character);
            charToStringMap[character] = name;
        }

        static CharClass()
        {
            stringToCharMap = new Dictionary<string, char>();
            DefineMapNamedLiteral(' ', "#\\space");
            DefineMapNamedLiteral('\n', "#\\newline");
            DefineMapNamedLiteral(0, "#\\eof");
        }
        public static bool NameToCharacter(string named, out char character)
        {
            return stringToCharMap.TryGetValue(named, out character);
        }

        public static string CharacterToName(char value)
        {
            if (value < charToStringMap.Length && charToStringMap[value] != null)
                return charToStringMap[value];

            return "#\\" + value.ToString();
        }

        #endregion
    }
}
