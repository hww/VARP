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

namespace VARP.Scheme.Stx
{
    using Data;
    using Primitives;
    using DataStructures;
    using VM;

    public sealed class CodegenEnvironemnt : Environment
    {
        public CodegenEnvironemnt() : base(null)
        {
            //DefinePrimitive("define", PrimitiveDefine.Expand);
            //DefinePrimitive("set!", PrimitiveSet.Expand);
            //DefinePrimitive("if", PrimitiveIf.Expand);
            //DefinePrimitive("cond", PrimitiveCond.Expand);
            //DefinePrimitive("lambda", PrimitiveLambda.Expand);
            //DefinePrimitive("begin", PrimitiveBegin.Expand);
            //DefinePrimitive("let", PrimitiveLet.Expand);

            //DefinePrimitive("=", PrimitiveArgs2.Expand);
            //DefinePrimitive("<", PrimitiveArgs2.Expand);
            //DefinePrimitive(">", PrimitiveArgs2.Expand);
            DefinePrimitive("+", Expand);
            //DefinePrimitive("-", PrimitiveArgs2.Expand);
            //DefinePrimitive("*", PrimitiveArgs2.Expand);
            //DefinePrimitive("/", PrimitiveArgs2.Expand);
            //DefinePrimitive("or", PrimitiveArgs2.Expand);
            //DefinePrimitive("and", PrimitiveArgs2.Expand);
            //DefinePrimitive("not", PrimitiveArgs1.Expand);
            //DefinePrimitive("display", PrimitiveArgsX.Expand);

            //DefinePrimitive("quote", QuotePrimitive.Expand);
            //DefinePrimitive("quaziquote", QuaziquotePrimitive.Expand);
        }
    }

 

}