﻿/* 
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

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace VARP.Scheme.Codegen
{
    using VM;
    using Data;
    using DataStructures;
    using Stx;


    public partial class CodeGenerator
    {
        private const int initialLiteralsSize = 10;
        private const int initialCodeSize = 10;

        private List<Value> Literals;
        private List<Template.UpValInfo> UpValues;
        private List<Template.ArgumentInfo> Values;
        private short ReqArgsNumber;
        private short OptArgsNumber;
        private short KeyArgsNumber;
        private short UpValueIdx;
        private short RestValueIdx;

        private List<Instruction> Code;

        public Template GetTemplate()
        {
            var template = new Template();

            template.Values = Values.ToArray();
            template.UpValues = UpValues.ToArray();
            template.ReqArgsNumber = ReqArgsNumber;
            template.OptArgsNumber = OptArgsNumber;
            template.KeyArgsNumber = KeyArgsNumber;
            template.UpValsNumber = UpValueIdx;
            template.RestValueIdx = RestValueIdx;
            template.Literals = Literals.ToArray();
            template.Code = Code.ToArray();
            template.FrameSize = SpMax + 1;
            template.SP = SpMin;

            Literals = null;
            UpValues = null;
            Values = null;
            Code = null;

            return template;
        }

        /// <summary>
        /// Lambda without arguments. It happens
        /// when we type expression like:
        /// (+ 1 2)
        /// </summary>
        public CodeGenerator()
        {
            Code = new List<Instruction>(initialCodeSize);
            Literals = new List<Value>(initialLiteralsSize);
            // Locals 
            Values = new List<Template.ArgumentInfo>();
            UpValues = new List<Template.UpValInfo>();

            OptArgsNumber = KeyArgsNumber = UpValueIdx = RestValueIdx = -1;
            SpMin = 0;
            SP = -1;
        }
        /// <summary>
        /// Defined lambda function
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="args"></param>
        public CodeGenerator(AstBinding[] args)
        { 
            Code = new List<Instruction>(initialCodeSize);
            Literals = new List<Value>(initialLiteralsSize);

            Values = new List<Template.ArgumentInfo>(args.Length);
            UpValues = new List<Template.UpValInfo>(args.Length);

            ReqArgsNumber = OptArgsNumber = KeyArgsNumber = 0;
            UpValueIdx = RestValueIdx = -1;

            foreach (var v in args)
            {

                // ---------------------------------------------
                // all values list. this will be frame size
                // ---------------------------------------------

                if (v is ArgumentBinding)
                {
                    var arg = v as ArgumentBinding;
                    switch (arg.ArgType)
                    {
                        case ArgumentBinding.Type.Required:
                            Values.Add(new Template.ArgumentInfo()
                            {
                                Name = v.Id.AsIdentifier(),
                                VarIdx = v.VarIdx,
                                LitIdx = -1
                            });
                            ReqArgsNumber++;
                            break;

                        case ArgumentBinding.Type.Optionals:
                            {
                                OptArgsNumber++;
                                // optional arguments are all the time pairs
                                // of identifier and initializer
                                var identifier = arg.Identifier;
                                var literal = -1;
                                if (arg.Initializer != null)
                                {
                                    if (arg.Initializer is AstLiteral)
                                        literal = DefineLiteral(new Value(arg.Initializer.GetDatum()));
                                    else
                                    {
                                        var code = GenerateCode(arg.Initializer);
                                        literal = DefineLiteral(new Value(code));
                                    }
                                }
                                else
                                {
                                    literal = -2;
                                }
                                // setup optional variable item
                                Values.Add(new Template.ArgumentInfo()
                                {
                                    Name = v.Id.AsIdentifier(),
                                    VarIdx = v.VarIdx,
                                    LitIdx = literal,
                                });
                            }
                            break;

                        case ArgumentBinding.Type.Key:
                            {
                                KeyArgsNumber++;
                                // optional arguments are all the time pairs
                                // of identifier and initializer
                                var identifier = arg.Identifier;
                                var literal = -1;
                                if (arg.Initializer != null)
                                {
                                    if (arg.Initializer is AstLiteral)
                                        literal = DefineLiteral(new Value(arg.Initializer.GetDatum()));
                                    else
                                    {
                                        var code = GenerateCode(arg.Initializer);
                                        literal = DefineLiteral(new Value(code));
                                    }
                                }
                                else
                                {
                                    literal = -2;
                                }
                                // setup optional variable item
                                Values.Add(new Template.ArgumentInfo()
                                {
                                    Name = v.Id.AsIdentifier(),
                                    VarIdx = v.VarIdx,
                                    LitIdx = literal,
                                });
                            }
                            break;

                        case ArgumentBinding.Type.Rest:
                            RestValueIdx = arg.VarIdx;
                            break;
                    }
                }
                else if (v is UpBinding)
                {
                    if (UpValueIdx < 0) UpValueIdx = v.VarIdx;
                    var arg = v as UpBinding;
                    // setup optional variable item
                    UpValues.Add(new Template.UpValInfo()
                    {
                        Name = arg.Identifier,
                        VarIdx = arg.VarIdx,
                        RefVarIndex = arg.RefVarIdx,
                        RefEnvIdx = arg.EnvOffset
                    });
                }

            }
            SpMin = (short)Values.Count;
            SP = (short)(SpMin - 1);
        }

        #region Temporary Variables

        private short SpMin;
        private short SpMax;
        private short sp;
        public short SP
        {
            get { return sp; }
            set
            {
                sp = value;
                SpMax = (short)Math.Max(value, SpMax);
            }
        }
        public short Pop()
        {
            return --SP;
        }
        public short Push()
        {
            return ++SP;
        }
        #endregion


        #region Varibales Methods

        /// <summary>
        /// Define up value 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public byte DefineUpValue(Symbol id, byte envIdx, byte varIdx)
        {
            var localid = DefineLocal(id);
            UpValues.Add(new Template.UpValInfo()
            {
                Name = id,
                RefEnvIdx = envIdx,
                RefVarIndex = varIdx,
                VarIdx = localid
            });
            return localid;
        }

        /// <summary>
        /// Create local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte DefineLocal(Symbol name)
        {
            var idx = ReferenceLocal(name);
            if (idx != 0xFF) return idx;
            // variable is not exists so we have to construct it
            Debug.Assert(Values.Count < 0xFF);
            var info = new Template.ArgumentInfo();
            info.Name = name;
            info.VarIdx = (byte)Values.Count;
            Values.Add(info);
            return info.VarIdx;
        }

        /// <summary>
        /// Find local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte ReferenceLocal(Symbol name)
        {
            for (var i = 0; i < Values.Count; i++)
                if (Values[i].Name == name) return (byte)i;
            return 0xFF;
        }
        #endregion

        #region Literals Methods

        /// <summary>
        /// Create literal and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DefineLiteral(Value val)
        {
            var idx = ReferenceLiteral(val);
            if (idx >= 0) return (ushort)idx;
            // literal is not found, define another one
            Literals.Add(val);
            return (ushort)(Literals.Count - 1);
        }

        /// <summary>
        /// Find literal and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int ReferenceLiteral(Value val)
        {
            for (var i = 0; i < Literals.Count; i++)
                if (Literals[i].Equals(val)) return i;
            return -1;
        }

        #endregion

        #region Program Counter Variables
        private int PC { get { return Code.Count; } }
        private int Jmp(int src, int dst) { return dst - src; }

        /// <summary>
        /// Make instruction of format A
        /// </summary>
        /// <param name="inst">instruction</param>
        private int AddOpcode(Instruction inst)
        {
            Code.Add(inst);
            return PC - 1;
        }
        /// <summary>
        /// Make instruction of format A
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        private int AddA(OpCode opcode, short a)
        {
            Code.Add(Instruction.MakeA(opcode, a));
            return PC - 1;
        }

        /// <summary>
        /// Make instruction of format A,B
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private int AddAB(OpCode opcode, short a, short b)
        {
            Code.Add(Instruction.MakeAB(opcode, a, b));
            return PC - 1;
        }

        /// <summary>
        /// Make instruction of format A,B,C
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        private int AddABC(OpCode opcode, short a, short b, short c)
        {
            Code.Add(Instruction.MakeABC(opcode, a, b, c));
            return PC - 1;
        }
        /// <summary>
        /// Make instruction of format A,BX
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        /// <param name="bx"></param>
        private int AddABX(OpCode opcode, short a, int bx)
        {
            Code.Add(Instruction.MakeABX(opcode, a, bx));
            return PC - 1;
        }

        /// <summary>
        /// Make instruction of format A,B or A,B,C
        /// </summary>
        /// <param name="code"></param>
        /// <param name="a"></param>
        /// <param name="sbx"></param>
        private int AddASBX(OpCode opcode, short a, int sbx)
        {
            Code.Add(Instruction.MakeASBX(opcode, a, sbx));
            return PC - 1;
        }
        #endregion
    }


}