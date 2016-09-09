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
using System.IO;

namespace VARP.Utils
{
    /// <summary>
    /// Allow to make peek with offset index
    /// EXAMPLE:
    /// char c = betterReader.PeekAt(2); 
    /// </summary>
    class BetterTextReader
    {
        TextReader reader;
        List<int> queue = new List<int>();

        public BetterTextReader(TextReader reader)
        {
            this.reader = reader;
        }
        /// <summary>
        /// Close stream
        /// </summary>
        public void Close()
        {
            reader.Close();
        }
        /// <summary>
        /// Peek symbol with offset
        /// Do not change pointer in stream
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int PeekAt(int offset)
        {
            while (queue.Count < (offset + 1))
                queue.Add(reader.Read());
            return queue[offset];
        }
        /// <summary>
        /// Read symbol from stream 
        /// and increment current pointer
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            if (queue.Count > 0)
            {
                var item = queue[0];
                queue.RemoveAt(0);
                return item;
            }
            else return reader.Read();
        }
    }
}