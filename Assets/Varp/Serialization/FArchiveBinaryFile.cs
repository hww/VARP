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
    public class FBinaryStreamReader : FArchive
    {

        public FBinaryStreamReader(Stream inStream, IOutputDevice inError) : base()
        {
            stream = inStream;
            error = inError;
            reader = new BinaryReader(stream);
        }
        ~FBinaryStreamReader()
        {
            Close();
            stream = null;
            error = null;
            reader = null;
        }
        public override bool AtEnd()
        {
            return Position() >= Lenght();
        }
        public override void Seek(long InPos)
        {
            if (stream == null) return;
            stream.Seek(InPos, SeekOrigin.Begin);
        }
        public override long Position()
        {
            if (stream == null) return -1;
            return stream.Position;
        }
        public override long Lenght()
        {
            if (stream == null) return -1;
            return stream.Length;
        }
        public override bool Close()
        {
            if (stream == null) return false;
            stream.Close();
            return true;
        }
        public override void Flush()
        {

        }

        protected Stream stream;
        protected BinaryReader reader;
        protected IOutputDevice error;

        public override void Ar(ref object obj)
        {
            var visited = obj as IArchived;
            if (visited == null)
                Debug.Assert(visited != null);
            else
                visited.Serialize(this);
        }
        public override void Ar(ref bool B) { B = reader.ReadBoolean(); }
        public override void Ar(ref char C) { C = reader.ReadChar(); }
        public override void Ar(ref sbyte B) { B = reader.ReadSByte(); }
        public override void Ar(ref byte B) { B = reader.ReadByte(); }
        public override void Ar(ref short S) { S = reader.ReadInt16(); }
        public override void Ar(ref ushort W) { W = reader.ReadUInt16(); }
        public override void Ar(ref int D) { D = reader.ReadInt32(); }
        public override void Ar(ref uint I) { I = reader.ReadUInt32(); }
        public override void Ar(ref float F) { F = reader.ReadSingle(); }
        public override void Ar(ref double F) { F = reader.ReadDouble(); }
    }
    // ==============================================================================================
    public class FBinaryStreamWriter : FArchive
    {
        public FBinaryStreamWriter(Stream inStream, IOutputDevice inError)
        {
            stream = inStream;
            error = inError;
            writer = new BinaryWriter(stream);
        }

        ~FBinaryStreamWriter()
        {
            Close();
            stream = null;
            error = null;
            writer = null;
        }
        public override bool AtEnd()
        {
            return false;
        }
        public override void Seek(long InPos)
        {
            if (stream == null) return;
            stream.Seek(InPos, SeekOrigin.Begin);
        }

        public override long Position()
        {
            if (stream == null) return 0;
            return stream.Position;
        }

        public override bool Close()
        {
            if (stream == null) return false;
            stream.Close();
            return true;
        }
        public override long Lenght()
        {
            if (stream == null) return -1;
            return stream.Length;
        }

        public override void Flush()
        {
            if (stream == null) return;
            stream.Flush();
        }


        protected Stream stream;
        protected BinaryWriter writer;
        protected IOutputDevice error;

        public override void Ar(ref object obj)
        {
            var visited = obj as IArchived;
            if (visited == null)
                Debug.Assert(visited != null);
            else
                visited.Serialize(this);
        }

        public override void Ar(ref bool v) { writer.Write(v); }
        public override void Ar(ref char v) { writer.Write(v); }
        public override void Ar(ref sbyte v) { writer.Write(v); }
        public override void Ar(ref byte v) { writer.Write(v); }
        public override void Ar(ref short v) { writer.Write(v); }
        public override void Ar(ref ushort v) { writer.Write(v); }
        public override void Ar(ref int v) { writer.Write(v); }
        public override void Ar(ref uint v) { writer.Write(v); }
        public override void Ar(ref float v) { writer.Write(v); }
        public override void Ar(ref double v) { writer.Write(v); }
    }
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