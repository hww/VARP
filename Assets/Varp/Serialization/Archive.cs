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

namespace VARP.Serialization
{
    public interface IArchived
    {
        void Serialize(FArchive ar);
    }

    public abstract class FArchive
    {
        // -------------------------------------------------------
        // Properties
        // -------------------------------------------------------

        // Status variables.
        protected int ArVer;
        protected bool ArIsLoading;
        protected bool ArIsSaving;
        protected bool ArIsPersistent;
        protected bool ArIsError;

        public int Ver { get { return ArVer; } }
        public bool IsLoading { get { return ArIsLoading; } }
        public bool IsSaving { get { return ArIsSaving; } }
        public bool IsPersistent { get { return ArIsPersistent; } }
        public bool IsError { get { return ArIsError; } }

        // -------------------------------------------------------
        // Constructor.
        // -------------------------------------------------------
        public FArchive()
        {
            ArVer = 0;
            ArIsLoading = false;
            ArIsSaving = false;
            ArIsPersistent = false;
            ArIsError = false;
        }

        // -------------------------------------------------------
        // Methods.
        // -------------------------------------------------------

        public abstract long Position();
        public abstract long Lenght();
        public abstract bool AtEnd();
        public abstract void Seek(long InPos);
        public abstract void Flush();
        public abstract bool Close();

        // -------------------------------------------------------
        // archivers.
        // -------------------------------------------------------

        public abstract void Ar(ref object obj);
        public abstract void Ar(ref bool v);
        public abstract void Ar(ref char v);
        public abstract void Ar(ref sbyte v);
        public abstract void Ar(ref byte v);
        public abstract void Ar(ref short v);
        public abstract void Ar(ref ushort v);
        public abstract void Ar(ref int v);
        public abstract void Ar(ref uint v);
        public abstract void Ar(ref float v);
        public abstract void Ar(ref double v);


    }


}
