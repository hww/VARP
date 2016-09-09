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

using UnityEngine;

namespace VARP.Timing
{
    /// <summary>
    /// Scaled Time
    /// 
    /// Allow to read @time and @deltaTime but those values depends on
    /// @timeScale property and @enabled flag.
    /// </summary>
    public class FScaledTime : FTickerObjectBase
    {
        bool enabled_;
        float deltaTime_;
        float timeScale_;
        float time_;

        // Constructors.
        public FScaledTime(FTicker inTicker = null) : base(0, inTicker)
        {
            enabled_ = true;
            timeScale = 1f;
        }
        public FScaledTime(bool inEnabled, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled_ = inEnabled;
            timeScale = 1f;
        }
        public FScaledTime(float inTimeScale, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled_ = true;
            timeScale = inTimeScale;
        }
        public FScaledTime(bool inEnabled, float inTimeScale, FTicker inTicker = null) : base(0, inTicker)
        {
            enabled_ = inEnabled;
            timeScale = inTimeScale;
        }

        // Methods.
        public float detaTime { get { return deltaTime_; } }
        public float time { get { return time_; } }
        public float timeScale { get { return timeScale_; } set { timeScale_ = value; } }
        public bool enabled { get { return enabled_; } set { enabled_ = value; } }

        // Private stuff
        protected override bool Tick(float DeltaTime)
        {
            deltaTime_ = enabled ? Time.deltaTime * timeScale_ : 0;
            time_ += deltaTime_;
            return true;
        }
    }
}
