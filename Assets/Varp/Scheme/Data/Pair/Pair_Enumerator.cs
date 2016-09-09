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
using System;
using System.Collections;

namespace VARP.Scheme.Data
{

    public sealed partial class Pair : SObject, ICollection<SObject>, IList<SObject>
    {
        private class PairEnumerator : IEnumerator<SObject>
        {

            Pair FirstPair, CurrentPair, LoopHead;

            bool IsVisitedLoopHead = false;
            bool IsVisitedImproperElement = false;

            public PairEnumerator(Pair firstPair)
            {
                this.FirstPair = firstPair;
                this.CurrentPair = null;
                this.LoopHead = firstPair.GetLoopHead();
            }


            #region IEnumerator Members
            void IDisposable.Dispose() { }
            public void Reset()
            {
                CurrentPair = null;
                IsVisitedLoopHead = false;
                IsVisitedImproperElement = false;
            }

            public SObject Current
            {
                get
                {
                    if (IsVisitedImproperElement)
                        return CurrentPair.Cdr;
                    return CurrentPair.Car;
                }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (IsVisitedImproperElement) return false;

                if (FirstPair == null || (CurrentPair != null && CurrentPair.Cdr == null))
                    return false;

                if (CurrentPair != null && IsVisitedLoopHead && CurrentPair.Cdr == LoopHead)
                    return false;

                if (CurrentPair == null)
                {
                    CurrentPair = FirstPair;
                }
                else
                {
                    if (CurrentPair.Cdr is Pair)
                        CurrentPair = (Pair)CurrentPair.Cdr;
                    else
                        IsVisitedImproperElement = true;
                }

                if (CurrentPair == LoopHead) IsVisitedLoopHead = true;

                return true;
            }

            #endregion
        }
    }
}
