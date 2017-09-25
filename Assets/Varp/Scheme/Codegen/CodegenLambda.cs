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

using System;
using System.Diagnostics;
using System.Collections.Generic;
using VARP.Scheme.Tokenizing;

namespace VARP.Scheme.Codegen
{
    using VM;
    using Data;
    using DataStructures;
    using Stx;


    public partial class CodeGenerator
    {
        private const int DEFAULT_VARIABLE_NUMBER = 16;
        private const int DEFAULT_LITERALS_NUMBER = 16;
        private const int DEFAULT_CODE_SIZE = 16;

        private List<Value> Literals;
        private List<VariableInfo> Values;
        private List<Instruction> Code;

        private List<Location> CodeDebug;

        private int ReqArgsNumber;
        private int OptArgsNumber;
        private int KeyArgsNumber;
        private int UpValueIdx;
        private int RestValueIdx;

        /// <summary>
        /// Lambda without arguments. It happens
        /// when we type expression like:
        /// (+ 1 2)
        /// </summary>
        public CodeGenerator(int valuesCount = DEFAULT_VARIABLE_NUMBER)
        {
            Values = new List<VariableInfo>(valuesCount);
            Literals = new List<Value>(DEFAULT_LITERALS_NUMBER);
            Code = new List<Instruction>(DEFAULT_CODE_SIZE);
            CodeDebug = new List<Location>(DEFAULT_CODE_SIZE);

            OptArgsNumber = KeyArgsNumber = 0;
            UpValueIdx = RestValueIdx = -1;
            SpMin = 0;
            SP = -1;
        }

        /// <summary>
        /// Defined lambda function
        /// </summary>
        /// <param name="args"></param>
        public void DefineArguments(AstBinding[] args)
        { 
            for (var idx=0; idx < args.Length; idx++)
            {
                var v = args[idx];
                // ---------------------------------------------
                // all values list. this will be frame size
                // ---------------------------------------------

                if (v is ArgumentBinding)
                {
                    var arg = v as ArgumentBinding;
                    switch (arg.ArgType)
                    {
                        case ArgumentBinding.Type.Required:
                            DefineLocal(v.Id.AsIdentifier(), -1);
                            ReqArgsNumber++;
                            break;

                        case ArgumentBinding.Type.Optionals:
                            {

                                // optional arguments are all the time pairs
                                // of identifier and initializer
                                var identifier = arg.Identifier;
                                var literal = -1;
                                if (arg.Initializer != null)
                                {
                                    if (arg.Initializer is AstLiteral)
                                    {
                                        literal = DefineLiteral(new Value(arg.Initializer.GetDatum()));
                                    }
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
                                Values.Add(new VariableInfo()
                                {
                                    Type = VariableType.Local,
                                    Name = v.Id.AsIdentifier(),
                                    LitIdx = literal,
                                });
                                OptArgsNumber++;
                            }
                            break;

                        case ArgumentBinding.Type.Key:
                            {

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

                                Values.Add(new VariableInfo()
                                {
                                    Type = VariableType.Local,
                                    Name = v.Id.AsIdentifier(),
                                    LitIdx = literal,
                                });
                                KeyArgsNumber++;
                            }
                            break;

                        case ArgumentBinding.Type.Rest:
                            RestValueIdx = idx;
                            Values.Add(new VariableInfo()
                            {
                                Type = VariableType.Local,
                                Name = v.Id.AsIdentifier(),
                                LitIdx = -1,
                            });
                            break;
                    }
                }
                else if (v is UpBinding)
                {
                    if (UpValueIdx < 0) UpValueIdx = idx;
                    var arg = v as UpBinding;
                    // setup optional variable item
                    DefineUpValue(arg.Identifier, arg.UpEnvIdx, arg.UpVarIdx);
                }
                else if (v is GlobalBinding)
                {
                    if (UpValueIdx < 0) UpValueIdx = idx;
                    var arg = v as GlobalBinding;
                    DefineGlobal(arg.Identifier, -1);
                }
                else
                    throw new System.Exception();

            }
            SpMin = (short)Values.Count;
            SP = (short)(SpMin - 1);
        }

        public Template GetTemplate()
        {
            var template = new Template();

            template.Variables = Values.ToArray();
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
            Values = null;
            Code = null;

            return template;
        }


        #region Temporary Variables

        private int SpMin;
        private int SpMax;

        private int sp;
        public int SP
        {
            get { return sp; }
            set { sp = value; if (sp > SpMax) SpMax = sp; }
        }

        public int Pop() { return --SP; }
        public int Push() { return ++SP; }

        #endregion

        #region Varibales Methods

        /// <summary>
        /// Define up value 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public int DefineUpValue(Symbol name, int upEnvIdx, int upVarIdx, Location location = null)
        {
            Values.Add(new VariableInfo()
            {
                Type = VariableType.UpValue,
                Name = name,
                UpEnvIdx = (short)upEnvIdx,
                UpVarIndex = (short)upVarIdx,
            });

            return Values.Count;
        }

        /// <summary>
        /// Create local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DefineLocal(Symbol name, int literalId = -1)
        {
            var idx = ReferenceLocal(name);
            if (idx >= 0) return idx;

            // variable is not exists so we have to construct it
            var info = new VariableInfo() { Type = VariableType.Local, Name = name, LitIdx = literalId };

            Values.Add(info);
            return Values.Count-1;
        }

        /// <summary>
        /// Create global variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DefineGlobal(Symbol name, int literalId = -1)
        {
            var idx = ReferenceLocal(name);
            if (idx >= 0) return idx;

            // variable is not exists so we have to construct it
            var info = new VariableInfo() { Type = VariableType.Global, Name = name, LitIdx = literalId };

            Values.Add(info);
            return Values.Count - 1;
        }

        /// <summary>
        /// Find local variable and returns it's ID 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int ReferenceLocal(Symbol name)
        {
            for (var i = 0; i < Values.Count; i++)
                if (Values[i].Name == name) return i;
            return -1;
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
        private int AddA(OpCode opcode, int a)
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
        private int AddAB(OpCode opcode, int a, int b)
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
        private int AddABC(OpCode opcode, int a, int b, int c)
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
        private int AddABX(OpCode opcode, int a, int bx)
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
        private int AddASBX(OpCode opcode, int a, int sbx)
        {
            Code.Add(Instruction.MakeASBX(opcode, a, sbx));
            return PC - 1;
        }

        #endregion
    }


}