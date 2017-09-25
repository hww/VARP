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

using JetBrains.Annotations;

namespace VARP.Scheme.VM
{
    using Data;
    using System;
    using System.Collections.Generic;
    using VARP.Scheme.Stx;

    public class Frame 
    {
        public Environment environment;

        public Frame parent;            //< pointer to parent frame
        public Template template;       //< pointer to template
        public Value[] Values;          //< register set
        public int FrameNum;            //< frame number in hierarhy
        public int PC;                  //< program counter
        public int SP;                  //< stack pointer

        public Frame(Frame parent, Template template)
        {
            this.parent = parent;
            this.template = template;
            Values = new Value[template.FrameSize];
            environment = parent == null ? SystemEnvironment.Top : parent.environment;
            SP = template.SP;
            FrameNum = parent == null ? 0 : parent.FrameNum + 1;
        }

        public Frame(Frame parent, Template template, Environment environment)
        {
            this.parent = parent;
            this.template = template;
            Values = new Value[template.FrameSize];
            this.environment = environment;
            SP = template.SP;
        }

        /// <summary>
        /// Return string of current location in the source code
        /// </summary>
        /// <returns></returns>
        public string GetLocationString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Get top frame of the environment
        /// </summary>
        /// <returns></returns>
        public Frame GetTopFrame()
        {
            var curent = this;
            while (curent.parent != null)
                curent = curent.parent;
            return curent;
        }

        /// <summary>
        /// Get top frame of the environment
        /// </summary>
        /// <returns></returns>
        public Frame GetFrame(int idx)
        {
            var curent = this;
            while (curent != null)
            {
                if (idx == 0) return curent;
                curent = curent.parent;
                idx--;
            }
            return null;
        }

        /// <summary>
        /// Return depth of the stack
        /// </summary>
        /// <returns></returns>
        public int GetDept()
        {
            return FrameNum-1;
        }

        /// <summary>
        /// Convert to the array
        /// First element is top
        /// </summary>
        /// <returns></returns>
        public Frame[] ToArray() 
        {
            var array = new Frame[GetDept()];
            var curent = this;
            while (curent != null)
            {
                array[curent.FrameNum] = curent;
                curent = curent.parent;
            }
            return array;
    }

        #region Variables Methods

        /// <summary>
        /// Find index of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal virtual int IndexOfVariable(Symbol name)
        {
            return template.IndexOfArgument(name);
        }

        /// <summary>
        /// Find index and frame of argument
        /// </summary>
        /// <param name="name">identifier</param>
        /// <returns>Binding or null</returns>
        internal virtual Frame IndexOfVariableRecursively(Symbol name, ref int frameIdx, ref int varIdx)
        {
            var curframe = this;

            while (curframe != null)
            {
                var curVarIndex = curframe.IndexOfVariable(name);
                if (curVarIndex >=0)
                {
                    varIdx = curVarIndex;
                    frameIdx = FrameNum - curframe.FrameNum;
                    return curframe;
                }
                curframe = curframe.parent;
            }

            return null;
        }

        #endregion
    }

}