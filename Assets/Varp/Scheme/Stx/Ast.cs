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

using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using REPL;
    using VM;

    /// <summary>
    /// This structure have to be used as shortcuts to already existing type such as
    /// Syntax and Pair. Use the data in existing data structure and do not make copy
    /// values without reason. It will make more loaded GC but at same time simpify code.
    /// </summary>
    public abstract class AST : SObject
    {
        /// <summary>
        /// This is location of the expression
        /// For instance the expression (+ 1 2) will have
        /// position at first open bracket
        /// But for literal x the position of expression is
        /// position of this literal
        /// 
        /// for example: 
        ///   expression: (+ 1 2) 
        ///   will be: ast('(')
        /// </summary>
        protected Syntax Expression;   

        public AST(Syntax syntax)
        {
            Expression = syntax;
        }

        /// <summary>
        /// Extract datum from this AST object
        /// </summary>
        /// <returns></returns>
        public Value GetDatum() { return GetDatum(Expression); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Value GetSyntax() { return new Value(Expression); }

        #region Datum Extractor Methods

        public static Value GetDatum(AST ast)
        {
            return ast.GetDatum();
        }
        public static Value GetDatum(Syntax syn)
        {
            return syn.GetDatum();
        }
        public static Value GetDatum(Value value)
        {
            if (value == null)
                return Value.Nil;
            if (value.IsAst)
                return GetDatum(value.AsAST());
            if (value.IsSyntax)
                return GetDatum(value.AsSyntax());
            if (value.IsLinkedList<Value>())
                return GetDatum(value.AsLinkedList<Value>());
            if (value.IsList<Value>())
                return GetDatum(value.AsList<Value>());
            return new Value(value);
        }
        public static Value GetDatum(LinkedList<Value> list)
        {
            var result = new LinkedList<Value>();
            if (list == null) return Value.Nil;
            foreach (var curent in list)
                result.AddLast(GetDatum(curent));
            return new Value(result);
        }
        public static Value GetDatum(List<Value> list)
        {
            var result = new List<Value>(list.Count);
            foreach (var curent in list)
                result.Add(curent.AsAST().GetDatum());
            return new Value(result);
        }

        #endregion

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string ToString() { return ValueString.ToString(GetDatum()); }
        public override abstract string Inspect();
        #endregion

        protected string GetLocationString()
        {
            if (Expression == null) return string.Empty;
            if (Expression.Location == null) return string.Empty;
            return string.Format(":{0}:{1}", Expression.Location.LineNumber, Expression.Location.ColNumber);
        }

        #region DebuggerDisplay 
        public override string DebuggerDisplay
        {
            get
            {
                try
                {
                    return string.Format(Inspector.Inspect(this));
                }
                catch (System.Exception ex)
                {
                    return string.Format("#<ast ispect-error='{0}'>", ex.Message);
                }
            }
        }
        #endregion

    }

    // literal e.g. 99 or #f
    public sealed class AstLiteral : AST
    {
        public readonly bool isSyntaxLiteral;

        public AstLiteral(Syntax stx, bool isSyntaxLiteral = false) : base(stx)
        {
            this.isSyntaxLiteral = isSyntaxLiteral;
        }
 
        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-lit{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }


    // the variable type
    public enum AstReferenceType
    {
        Local,
        Global,
        UpValue
    }

    // variable reference  e.g. x
    public sealed class AstReference : AST
    {
        /// <summary>
        /// Type of this reference
        /// </summary>
        public AstReferenceType ReferenceType;

        /// <summary>
        /// index of the argument (local var index)
        /// </summary>
        public byte VarIdx;

        /// <summary>
        /// index of the referenced environment
        /// 0 for local
        /// -1 for global
        /// </summary>
        public short UpEnvIdx = -1;

        /// <summary>
        /// FrameNum of variable in the referenced environment
        /// -1 for global
        /// </summary>
        public short UpVarIdx = -1;

        /// <summary>
        /// Create new reference
        /// </summary>
        /// <param name="syntax">reference's syntax</param>
        /// <param name="argIdx">index in local scope</param>
        /// <param name="upEnvIdx">index (relative offset) of environment</param>
        /// <param name="upVarIdx">index of variable inside referenced environment</param>
        public AstReference(Syntax syntax, AstReferenceType type, int argIdx) : base(syntax)
        {
            ReferenceType = type;
            VarIdx = (byte)argIdx;
        }

        /// <summary>
        /// Create new reference
        /// </summary>
        /// <param name="syntax">reference's syntax</param>
        /// <param name="argIdx">index in local scope</param>
        /// <param name="upEnvIdx">index (relative offset) of environment</param>
        /// <param name="upVarIdx">index of variable inside referenced environment</param>
        public AstReference(Syntax syntax, AstReferenceType type, int argIdx, int upEnvIdx, int upVarIdx) : base(syntax)
        { 
            ReferenceType = type;
            VarIdx = (byte)argIdx;
            UpEnvIdx = (short)upEnvIdx;
            UpVarIdx = (short)upVarIdx;
        }

        public bool IsLocal { get { return ReferenceType == AstReferenceType.Local; } }
        public bool IsGlobal { get { return ReferenceType == AstReferenceType.Global; } }
        public bool IsUpValue { get { return ReferenceType == AstReferenceType.UpValue; } }

        public Symbol Identifier { get { return Expression.AsIdentifier(); } }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-ref{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // variable assignment e.g. (set! x 99)
    public sealed class AstSet : AST
    {
        public Syntax Variable;             // x   
        public AST Value;                   // 99
        public int VarIdx;                  // index of variable
        public int UpEnvIdx;                // index of environment 
        public int UpVarIdx;                // index of variables

        public AstSet(Syntax syntax, Syntax variable, AST value, int varIdx, int refEnvIdx, int refVarIdx) : base(syntax)
        {
            VarIdx = (byte)varIdx;
            UpEnvIdx = (short)refEnvIdx;
            UpVarIdx = (short)refVarIdx;

            Variable = variable;
            Value = value;
        }
        public bool IsGlobal { get { return UpVarIdx < 0; } }
        public bool IsUpValue { get { return UpEnvIdx > 0; } }
        public Symbol Identifier { get { return Variable.AsIdentifier(); } }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-set{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // conditional e.g. (if 1 2 3)
    public sealed class AstConditionIf : AST
    {
        private Syntax Keyword;
        public AST condExpression;        // 1
        public AST thenExperssion;        // 2
        public AST elseExpression;        // 3

        public AstConditionIf(Syntax syntax, Syntax keyword, AST cond, AST then, AST els) : base(syntax)
        {
            Keyword = keyword;
            condExpression = cond;
            thenExperssion = then;
            elseExpression = els;
        }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-if{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // conditional e.g. (cond (() .. ) (() ...) (else ...))
    public sealed class AstCondition : AST
    {
        private Syntax Keyword;
        public LinkedList<Value> Conditions;     //< list of pairs
        public LinkedList<Value> ElseCase;       //< else condition

        public AstCondition(Syntax syntax, Syntax keyword, LinkedList<Value> conditions, LinkedList<Value> elseCase) : base(syntax)
        {
            Keyword = keyword;
            Conditions = conditions;
            ElseCase = elseCase;
        }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-cond{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

    // primitive op e.g. (+ 1 2)
    public sealed class AstPrimitive : AST
    {
        public Syntax Identifier;
        public LinkedList<Value> Arguments;
        public AstPrimitive(Syntax syntax, Syntax identifier, LinkedList<Value> arguments) : base(syntax)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-prim{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // application e.g. (f 1 2)
    public sealed class AstApplication : AST
    {
        public LinkedList<Value> list;
        public AstApplication(Syntax syntax, LinkedList<Value> expression) : base(syntax)
        {
            list = expression;
        }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-app{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

    // lambda expression   e.g. (lambda(x) x)
    public sealed class AstLambda : AST
    {
        private Syntax Keyword;                      // (<lambda> (...) ...)
        public AstBinding[] ArgList;                 // (lambda <(...)> ) TODO can be replace to reference to Environment!
        public LinkedList<Value> BodyExpression;     // (lambda (...) <...>)

        public AstLambda(Syntax syntax, Syntax keyword, Environment environment, LinkedList<Value> expression) : base(syntax)
        {
            ArgList = environment.ToAstArray();
            BodyExpression = expression;
            if (keyword.GetDatum() == Symbol.LAMBDA)
                Keyword = keyword;
            else
                Keyword = new Syntax(Symbol.LAMBDA, keyword.Location);
        }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-lam{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

    // sequence e.g. (begin 1 2)
    public sealed class AstSequence : AST
    {
        private Syntax Keyword;
        public LinkedList<Value> BodyExpression;
        public AstSequence(Syntax syntax, Syntax keyword, LinkedList<Value> expression) : base(syntax)
        {
            Keyword = keyword;
            BodyExpression = expression;
        }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-seq{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

}