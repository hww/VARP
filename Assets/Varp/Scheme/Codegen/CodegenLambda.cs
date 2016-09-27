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

namespace VARP.Scheme.Codegen
{
    using VM;
    using Data;
    using DataStructures;
    using Stx;


    public partial class CodeGenerator
    {
        const int initialLiteralsSize = 10;
        const int initialCodeSize = 10;

        private List<Value> Literals;

        private List<Template.UpValInfo> UpValues;
        private List<Template.LocalVarInfo> Values;
        private List<Template.KeyVarInfo> OptVals;
        private List<Template.KeyVarInfo> KeyVals;

        private int OptValueIdx;
        private int KeyValueIdx;
        private int UpValueIdx;
        private int RestValueIdx;

        private List<Instruction> Code;

        public Template GetTemplate()
        {
            Template template = new Template();

            template.Values = Values.ToArray();
            template.UpValues = UpValues.ToArray();
            template.OptVals = OptVals.ToArray();
            template.KeyVals = KeyVals.ToArray();
            template.OptValueIdx = OptValueIdx;
            template.KeyValueIdx = KeyValueIdx;
            template.UpValueIdx = UpValueIdx;
            template.RestValueIdx = RestValueIdx;
            template.Literals = Literals.ToArray();
            template.Code = Code.ToArray();

            Literals = null;
            UpValues = null;
            Values = null;
            OptVals = null;
            KeyVals = null;
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
            Values = new List<Template.LocalVarInfo>();
            OptVals = new List<Template.KeyVarInfo>();
            KeyVals = new List<Template.KeyVarInfo>();
            UpValues = new List<Template.UpValInfo>();

            OptValueIdx = KeyValueIdx = UpValueIdx = RestValueIdx = -1;
            TempIndex = 0;
        }
        /// <summary>
        /// Defined lambda function
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="args"></param>
        public CodeGenerator(Stx.Binding[] args)
        { 
            Code = new List<Instruction>(initialCodeSize);
            Literals = new List<Value>(initialLiteralsSize);

            Values = new List<Template.LocalVarInfo>(args.Length);
            OptVals = new List<Template.KeyVarInfo>(args.Length);
            KeyVals = new List<Template.KeyVarInfo>(args.Length);
            UpValues = new List<Template.UpValInfo>(args.Length);

            foreach (var v in args)
            {

                // ---------------------------------------------
                // all values list. this will be frame size
                // ---------------------------------------------
                Values.Add(new Template.LocalVarInfo()
                {
                    Name = v.Id.AsIdentifier(),
                    ArgIndex = v.Index
                });

                if (v is ArgumentBinding)
                {
                    ArgumentBinding arg = v as ArgumentBinding;
                    switch (arg.ArgType)
                    {
                        case ArgumentBinding.Type.Required:

                            break;
                        case ArgumentBinding.Type.Optionals:
                            {
                                // optional arguments are all the time pairs
                                // of identifier and initializer
                                Symbol identifier = arg.Identifier;
                                Template code = GenerateCode(arg.Initializer);
                                int literal = DefineLiteral(new Value(code));
                                // setup optional variable item
                                OptVals.Add(new Template.KeyVarInfo()
                                {
                                    Name = identifier,
                                    LitIndex = literal,
                                    ArgIndex = v.Index
                                });
                            }
                            break;
                        case ArgumentBinding.Type.Key:
                            {
                                // optional arguments are all the time pairs
                                // of identifier and initializer
                                Symbol identifier = arg.Identifier;
                                Template code = GenerateCode(arg.Initializer);
                                int literal = DefineLiteral(new Value(code));
                                // setup optional variable item
                                KeyVals.Add(new Template.KeyVarInfo()
                                {
                                    Name = identifier,
                                    LitIndex = literal,
                                    ArgIndex = v.Index
                                });
                            }
                            break;

                        case ArgumentBinding.Type.Rest:
                            RestValueIdx = arg.Index;
                            break;
                    }
                }
                else if (v is UpBinding)
                {
                    UpBinding arg = v as UpBinding;
                    // setup optional variable item
                    UpValues.Add(new Template.UpValInfo()
                    {
                        Name = arg.Identifier,
                        ArgIndex = arg.Index,
                        VarIndex = arg.Refrence.Index,
                        EnvIndex = arg.EnvIndex
                    });
                }

            }
            TempIndex = (ushort)Values.Count;
        }

        void GetIdentifierAndInitializer(Value value, out Symbol identifier, out Template code)
        {
            // optional arguments are all the time pairs
            // of identifier and initializer
            ValuePair pair = value.AsValuePair();
            // get identifier
            Syntax syn = pair.Item1.AsSyntax();
            identifier = syn.AsIdentifier();
            // get initializer
            AST ast = pair.Item2.AsAST();
            code = GenerateCode(ast);
        }

        #region Temporary Variables

        private ushort maxTempVar;
        private ushort curTempVar;
        public ushort TempIndex
        {
            get { return curTempVar; }
            set
            {
                maxTempVar = System.Math.Max(curTempVar = value, maxTempVar);
            }
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
            byte localid = DefineLocal(id);
            UpValues.Add(new Template.UpValInfo()
            {
                Name = id,
                EnvIndex = envIdx,
                VarIndex = varIdx,
                ArgIndex = localid
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
            byte idx = ReferenceLocal(name);
            if (idx != 0xFF) return idx;
            // variable is not exists so we have to construct it
            Debug.Assert(Values.Count < 0xFF);
            Template.LocalVarInfo info = new Template.LocalVarInfo();
            info.Name = name;
            info.ArgIndex = (byte)Values.Count;
            Values.Add(info);
            return info.ArgIndex;
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
            int idx = ReferenceLiteral(val);
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
        private int AddA(OpCode opcode, ushort a)
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
        private int AddAB(OpCode opcode, ushort a, ushort b)
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
        private int AddABC(OpCode opcode, ushort a, ushort b, ushort c)
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
        private int AddABX(OpCode opcode, ushort a, int bx)
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
        private int AddASBX(OpCode opcode, ushort a, int sbx)
        {
            Code.Add(Instruction.MakeASBX(opcode, a, sbx));
            return PC - 1;
        }
        #endregion
    }


}