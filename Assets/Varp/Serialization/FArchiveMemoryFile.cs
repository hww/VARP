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
using UnityEngine;
using VARP.Logging;

namespace VARP.Serialization
{
    // ==============================================================================================
    public class FMemoryStreamReader : FBinaryStreamReader
    {
        public FMemoryStreamReader(IOutputDevice inError) : base(new MemoryStream(), inError)
        {

        }
        public FMemoryStreamReader(int capacity, IOutputDevice inError) : base(new MemoryStream(capacity), inError)
        {

        }
        public FMemoryStreamReader(byte[] buffer, IOutputDevice inError) : base(new MemoryStream(buffer), inError)
        {

        }
        public FMemoryStreamReader(byte[] buffer, int index, int count, IOutputDevice inError) : base(new MemoryStream(buffer, index, count), inError)
        {

        }

    }
    // ==============================================================================================
    public class FMemoryStreamWriter : FBinaryStreamWriter
    {
        public FMemoryStreamWriter(IOutputDevice inError) : base(new MemoryStream(), inError)
        {

        }
        public FMemoryStreamWriter(int capacity, IOutputDevice inError) : base(new MemoryStream(capacity), inError)
        {

        }
        public FMemoryStreamWriter(byte[] buffer, IOutputDevice inError) : base(new MemoryStream(buffer), inError)
        {

        }
        public FMemoryStreamWriter(byte[] buffer, int index, int count, IOutputDevice inError) : base(new MemoryStream(buffer, index, count), inError)
        {

        }
    }

}