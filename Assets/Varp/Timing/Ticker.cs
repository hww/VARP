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

using System.Diagnostics;

namespace VARP.Timing
{

    using DataStructures;

    public class FTicker
    {
        #region Singletone

        static FTicker coreTicker;
        public static FTicker GetCoreTicker()
        {
            if (coreTicker == null) coreTicker = new FTicker();
            return coreTicker;
        }

        #endregion

        // @return true if have to be fired again or false to terminate
        // @deltaTime 
        public delegate bool FTickerDelegate(float deltaTime);

        // Single delegate item
        public class FElement
        {
            public LinkedListNode<FElement> Link;
            // Time that this delegate must not fire before
            public double FireTime;
            // Delay that this delegate was scheduled with. Kept here so that if the delegate returns true, we will reschedule it.
            public float DelayTime;
            // Delegate to call
            FTickerDelegate TheDelegate;

            // This is the ctor that the code will generally use. 
            public FElement(double inFireTime, float inDelayTime, FTickerDelegate inDelegate, object inDelegateHandle = null)
            {
                DelayTime = inDelayTime;
                FireTime = inDelayTime;
                TheDelegate = inDelegate;
            }

            // Invoke the delegate if possible 
            public bool Tick(float deltaTime)
            {
                if (TheDelegate == null) return false;
                if (TheDelegate(deltaTime)) return true;
                TheDelegate = null; // terminate
                return false;
            }

            public bool Equals(FTickerDelegate inDelegate)
            {
                return TheDelegate == inDelegate;
            }

            public bool EqualsHandle(int inHandle)
            {
                return GetHashCode() == inHandle;
            }

            public void Terminate()
            {
                TheDelegate = null;
            }

            public bool IsTerminated { get { return TheDelegate == null; } }
        };
        // @inDelegae the function will be fired
        // @inDelay delay before fire
        // @handle can be used later to find this delegate
        public int AddTicker(FTickerDelegate inDelegate, float inDelay)
        {
            FElement e = new FElement(currentTime + inDelay, inDelay, inDelegate);
            elements.AddFirst(e);
            return e.GetHashCode();
        }

        public void RemoveTicker(int inHandle)
        {
            foreach (var el in elements)
            {
                if (el.EqualsHandle(inHandle))
                    el.Terminate();
            }
        }
        public void RemoveTicker(FTickerDelegate inDelegate)
        {
            foreach (var el in elements)
                if (el.Equals(inDelegate))
                    el.Terminate();

        }

        public void Tick(float deltaTime)
        {
            lock (lockObject)
            {
                // Do not call it more that once per frame
                if (oncePerFrame.IsNotOnce) return;
                // Benchmarking
                var timer = Stopwatch.StartNew();
                isInTick = true;
                currentTime += deltaTime;

                var element = elements.First;
                while (element != null)
                {
                    // just in case deleting the element, check who is next
                    var next = element.Next;
                    // optionally: set current element for some of side effect tests
                    currentElement = element.Value;
                    // Tick
                    if (currentElement.Tick(deltaTime))
                        currentElement.FireTime = currentTime + currentElement.DelayTime;
                    else
                        currentElement.Link.Remove();
                    element = next;
                }
                // Benchmarking end
                timer.Stop();
                totalTimeMicroseconds = timer.ElapsedMilliseconds;
            }
        }

        // --------------------------------------------------------------------

        private object lockObject;          //< Lock object
        private OncePerFrame oncePerFrame;  //< Last frame count (prevent call twice in frame)
        private double currentTime;         //< Current time of the ticker
        private bool isInTick;              //< State to track whether CurrentElement is valid. 
        private FElement currentElement;    //< Current element being ticked (only valid during tick).
        private long totalTimeMicroseconds; //< Time of single invoke. Used for benchmarking
        // List of delegates
        private LinkedList<FElement> elements = new LinkedList<FElement>();
    }

    // The base class for objects which have to be called time to time.
    public class FTickerObjectBase
    {
        // Constructor
        //
        // @param InDelay Delay until next fire; 0 means "next frame"
        // @param Ticker the ticker to register with. Defaults to FTicker::GetCoreTicker().
        public FTickerObjectBase(float InDelay = 0.0f, FTicker inTicker = null)
        {
            Ticker = (inTicker == null) ? FTicker.GetCoreTicker() : inTicker;
            TickHandle = Ticker.AddTicker(Tick, InDelay);
        }

        /** Virtual destructor. */
        ~FTickerObjectBase()
        {
            if (Ticker != null) Ticker.RemoveTicker(TickHandle);
            Ticker = null;
        }

        // Pure virtual that must be overloaded by the inheriting class.
        //
        // @param DeltaTime	time passed since the last call.
        // @return true if should continue ticking
        protected virtual bool Tick(float DeltaTime) { return false; }


        // Ticker to register with 
        private FTicker Ticker;
        // Delegate for callbacks to Tick
        private int TickHandle;
    };
}
