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

using System.IO;

namespace VARP.Scheme.Tokenizing
{
    public sealed class TokenReader
    {
        /// <summary>
        /// Constructor that can be used by subclasses that don't want to provide tokens via a TextReader
        /// </summary>
        protected TokenReader() { }

        /// <summary>
        /// Constructs a token stream that reads token from the given TextReader
        /// </summary>
        /// <param name="textStream">The stream to read tokens from</param>
        public TokenReader(TextReader textStream, string filePath)
        {
            stream = textStream;
            this.filePath = filePath;

            charNumber = 0;
            lineNumber = 1;
            colNumber = 0;
        }

        private TextReader stream;

        #region Tracking where we are in the stream

        private const int contextLen = 8;

        private int charNumber;
        private int lineNumber;
        private int colNumber;
        private string filePath;

        /// <summary>
        /// Peeks at the next character from the stream
        /// </summary>
        public int Peek()
        {
            return stream.Peek();
        }

        /// <summary>
        /// Reads the next character from the stream
        /// </summary>
        public int Read()
        {
            var chr = stream.Read();

            if (chr != -1)
            {
                charNumber++;
                colNumber++;
            }

            if (chr == '\n')
            {
                lineNumber++;
                colNumber = 0;
            }

            return chr;
        }

        public string FilePath { get { return filePath; } }
        public int CharNumber { get { return charNumber; } }
        public int LineNumber { get { return lineNumber; } }
        public int ColNumber { get { return colNumber; } }


        #endregion
    }
}