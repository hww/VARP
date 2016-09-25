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

    public sealed class Template : Inspectable
    {
        internal struct UpValInfo
        {
            public Symbol Name;     //< variable name
            public byte ArgIndex;   //< variable index
            public byte EnvIndex;   //< the environment index
            public byte VarIndex;   //< index of the variable in frame
        }
        internal struct LocalVarInfo
        {
            public Symbol Name;                 //< variable name
            public byte ArgIndex;               //< variable index
        }

        internal struct KeyVarInfo
        {
            public Symbol Name;                 //< variable name
            public byte ArgIndex;               //< variable index
            public int LitIndex;                //< initializer
        }

        internal Instruction[] Code;            //< code sequence
        internal Location[] Locations;          //< the location in source code
        internal Value[] Literals;              //< list of literals, there will be child templates
        internal LocalVarInfo[] Values;         //< local vars info, include required, and optional
        internal UpValInfo[] UpValues;          //< up-values info
        internal KeyVarInfo[] KeyVals;          //< local vars info
        internal KeyVarInfo[] OptVals;          //< local vars info
        internal int OptValueIdx;               //< index of first element
        internal int KeyValueIdx;               //< index of first element
        internal int UpValueIdx;                //< index of first element
        internal int RestValueIdx;              //< index of first element
        public Template()
        {
            this.OptValueIdx = -1;
            this.KeyValueIdx = -1;
            this.UpValueIdx = -1;
            this.RestValueIdx = -1;
        }

        public Template(Value[] literals, Instruction[] code)
        {
            this.Literals = literals;
            this.Code = code;
            this.OptValueIdx = -1;
            this.KeyValueIdx = -1;
            this.UpValueIdx = -1;
            this.RestValueIdx = -1;
        }


        public string Inspect()
        {
            return Inspect(0);
        }

        public string Inspect(int ident)
        {
            string sident = new string(' ', ident * 2);

            StringBuilder sb = new StringBuilder();
            sb.Append(sident);
            sb.AppendLine("Template");
            /// arguments ///
            sb.Append(sident);
            sb.Append("  arguments:");
            foreach (var v in Values)
            {
                sb.Append(" ");
                sb.Append(v.Name.ToString());
            }
            sb.AppendLine();
            /// &optionals ///
            sb.Append(sident);
            sb.Append("  optionals:");
            foreach (var v in OptVals)
            {
                sb.Append(" ");
                sb.Append(v.Name.ToString());
                sb.Append(":");
                sb.Append(v.LitIndex.ToString());
            }
            sb.AppendLine();
            /// &keys ///
            sb.Append(sident);
            sb.Append("  key:");
            foreach (var v in OptVals)
            {
                sb.Append(" ");
                sb.Append(v.Name.ToString());
                sb.Append(":");
                sb.Append(v.LitIndex.ToString());
            }
            /// &rest ///
            if (RestValueIdx >= 0)
            {
                sb.AppendLine();
                sb.Append(sident);
                sb.Append("  rest: ");
                sb.Append(Values[RestValueIdx].Name.ToString());
            }
            sb.AppendLine();
            /// code ///
            sb.Append(sident);
            sb.AppendLine("  code:");
            int pc = 0;
            foreach (var v in Code)
            {
                sb.Append(sident);
                sb.Append(string.Format("  [{0}] ", pc));
                sb.Append(Code[pc++].ToString());
                sb.AppendLine();
            }


            sb.Append(sident);
            sb.Append("  literals:");
            sb.AppendLine();
            string lident = new string(' ', (ident + 1) * 2);
            int lidx = 0;
            foreach (var v in Literals)
            {
                sb.Append(lident);
                if (v.Is<Template>())
                {
                    sb.AppendLine(string.Format("  [{0}] -->", lidx++));
                    sb.Append(v.As<Template>().Inspect(ident + 2));
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(string.Format("  [{0}] ", lidx++));
                    sb.Append(v.ToString());
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

    }
}