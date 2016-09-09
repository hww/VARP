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
 namespace VARP.Utils
{
    public class BetterStringBuilder
    {

        private char[] Buffer;
        private int BufferPos;

        private string StringCache;

        public BetterStringBuilder(int capacity)
        {
            this.Buffer = new char[capacity];
        }

        /// <summary>
        /// Append string to the buffer
        /// </summary>
        /// <param name="c">Character to add</param>
        public void Append(char c)
        {
            Buffer[BufferPos++] = c;
            StringCache = null;
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        public void Clear()
        {
            BufferPos = 0;
            StringCache = null;
        }

        /// <summary>
        /// Get type of buffer
        /// </summary>
        public int Size
        {
            get { return BufferPos; }
        }

        /// <summary>
        /// Get string of the buffer
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            if (StringCache != null)
                return StringCache;
            return StringCache = new string(Buffer, 0, BufferPos);
        }
    }

}
