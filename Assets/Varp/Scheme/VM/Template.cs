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

namespace VARP.Scheme.VM
{
    using Data;
    using REPL;
    using Stx;
    using System.Text;
    using Tokenizing;

    internal enum VariableType : byte
    {
        Local,
        Global,
        UpValue
    }

    internal struct VariableInfo
    {
        public VariableType Type;           //< up value type
        public Symbol Name;                 //< variable name
        public short UpVarIndex;            //< index of variable in referenced environment 
        public short UpEnvIdx;              //< index of referenced environment 
        public int LitIdx;                  //< initializer: -1 for required

        public bool IsLocal { get { return Type == VariableType.Local; } }
        public bool IsGlobal { get { return Type == VariableType.Global; } }
        public bool IsUpvalue { get { return Type == VariableType.UpValue; } }
    }

    public sealed class Template : Inspectable
    {
        internal Instruction[] Code;            //< code sequence
        internal VariableInfo[] Variables;      //< local vars info, include required, and optional
        internal Value[] Literals;              //< list of literals, there will be child templates
        internal Location[] CodeDbg;            //< the location in source code

        internal int ReqArgsNumber;             //< quantity of arguments
        internal int OptArgsNumber;             //< quantity of arguments
        internal int KeyArgsNumber;             //< quantity of arguments
        internal int UpValsNumber;              //< quantity of arguments
        internal int RestValueIdx;              //< index of first element
        internal int SP;                        //< stack pointer position
        internal int FrameSize;                 //< full frame size

        public Template()
        {
            ReqArgsNumber = 0;
            OptArgsNumber = 0;
            KeyArgsNumber = 0;
            UpValsNumber = 0;
            RestValueIdx = -1;
            SP = -1;
        }

        public Template(Value[] literals, Instruction[] code)
        {
            Literals = literals;
            Code = code;
            ReqArgsNumber = 0;
            OptArgsNumber = 0;
            KeyArgsNumber = 0;
            UpValsNumber = 0;
            RestValueIdx = -1;
            SP = -1;
        }

        public string Inspect()
        {
            return Inspect(0);
        }

        public string Inspect(int ident)
        {
            var sident = new string(' ', ident * 4);

            var sb = new StringBuilder();
            sb.Append(sident);
            sb.AppendFormat("Template: args: {0} frame: {1}\n", Variables.Length, FrameSize);
            /////////////////
            /// arguments ///
            /////////////////
            sb.Append(sident);
            sb.Append("|  arguments:");
            foreach (var v in Variables)
            {
                if (v.IsLocal)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                    switch (v.LitIdx)
                    {
                        case -1:
                            break;
                        case -2:
                            sb.Append(":#f");
                            break;
                        default:
                            sb.Append(":");
                            sb.Append(v.LitIdx.ToString());
                            break;
                    }
                }
            }
            /////////////
            /// &rest ///
            /////////////
            if (RestValueIdx >= 0)
            {
                sb.Append(" &rest: ");
                sb.Append(Variables[RestValueIdx].Name.ToString());
            }
            sb.AppendLine();
            ///////////////
            /// &upvals ///
            ///////////////
            sb.Append(sident);
            sb.Append("|  upvalues:");
            foreach (var v in Variables)
            {
                if (v.IsGlobal)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                }
                else if (v.IsUpvalue)
                {
                    sb.Append(" ");
                    sb.Append(v.Name.ToString());
                    sb.Append(":");
                    sb.Append(v.UpEnvIdx);
                    sb.Append(":");
                    sb.Append(v.UpVarIndex);
                }
            }
            sb.AppendLine();
            ///////////
            /// &sp ///
            ///////////
            sb.Append(sident);
            sb.Append("|  temp:");
            for (var i = SP; i < FrameSize; i++)
                sb.AppendFormat(" T{0}", i);
            sb.AppendLine();
            ////////////
            /// code ///
            ////////////
            sb.Append(sident);
            sb.AppendLine("|  code:");
            var pc = 0;
            foreach (var v in Code)
            {
                sb.Append(sident);
                sb.Append(string.Format("|  [{0}] ", pc));
                sb.Append(Code[pc++].ToString());
                sb.AppendLine();
            }

            if (Literals.Length > 0)
            {
                sb.Append(sident);
                sb.Append("|  literals:");
                sb.AppendLine();
                var lident = new string(' ', (ident + 1) * 4);
                var lidx = 0;
                foreach (var v in Literals)
                {
                    sb.Append(lident);
                    if (v.Is<Template>())
                    {
                        sb.AppendLine(string.Format("  [{0}] -->", lidx++));
                        sb.Append(v.As<Template>().Inspect(ident + 2));
       
                    }
                    else
                    {
                        sb.Append(string.Format("  [{0}] ", lidx++));
                        sb.Append(v.ToString());
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Find index of the argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOfArgument(Symbol name)
        {
            for (var i = 0; i < Variables.Length; i++)
            {
                if (Variables[i].Name == name)
                    return i;
            }
            return -1;
        }


        internal VariableInfo GetVariable(int idx)
        {
            return Variables[idx];
        }

        internal Location GetCodeDbg(int idx)
        {
            if (CodeDbg == null) return null;
            return CodeDbg[idx];
        }

        internal void GetUpValue(int idx, ref int envIdx, ref int varIdx)
        {
            envIdx = Variables[idx].UpEnvIdx;
            varIdx = Variables[idx].UpVarIndex;
        }
    }
}