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

using System.Collections.Generic;

namespace VARP.Scheme.Stx
{
    using DataStructures;
    using Data;
    using Exception;
    using REPL;
    using System.Diagnostics;

    public interface IDatum
    {
        Value GetDatum();
    }
    /// <summary>
    /// This structure have to be used as shortcuts to already existing type such as
    /// Syntax and Pair. Use the data in existing data structure and do not make copy
    /// values without reason. It will make more loaded GC but at same time simpify code.
    /// </summary>
    public abstract class AST : ValueClass
    {
        /// <summary>
        /// This is location of the expression
        /// For instance the expression (+ 1 2) will have
        /// position at first open bracket
        /// But for literal x the position of expression is
        /// position of this literal
        /// </summary>
        protected Syntax Expression;   //< for expression (+ 1 2) will be "("

        public AST(Syntax syntax)
        {
            this.Expression = syntax;
        }

        /// <summary>
        /// Extract datum from this AST object
        /// </summary>
        /// <returns></returns>
        public abstract Value GetDatum();

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
            if (value == null) return Value.Nill;
            if (value.IsAST)
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
            LinkedList<Value> result = new LinkedList<Value>();
            if (list == null) return Value.Nill;
            foreach (Value curent in list)
                result.AddLast(GetDatum(curent));
            return new Value(result);
        }
        public static Value GetDatum(List<Value> list)
        {
            List<Value> result = new List<Value>(list.Count);
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
    }

    // literal e.g. 99 or #f
    public sealed class AstLiteral : AST
    {
        public AstLiteral(Syntax stx) : base(stx)
        {
        }
        public override Value GetDatum() { return GetDatum(Expression); }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-lit{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }


    // variable reference  e.g. x
    public sealed class AstReference : AST
    {
        public Binding Binding;
        public AstReference(Syntax syntax, Binding binding) : base(syntax)
        {
            this.Binding = binding;
        }
        public override Value GetDatum() { return GetDatum(Expression); }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-ref{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // variable assignment e.g. (set! x 99)
    public sealed class AstSet : AST
    {
        private Syntax Keyword;              // set!                                     
        private Syntax Variable;             // x   
        private AST Value;                   // 99
        private Binding Binding;      //
        public AstSet(Syntax syntax, Syntax keyword, Syntax variable, AST value, Binding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override Value GetDatum() { return new Value(ValueLinkedList.FromArguments(Expression.GetDatum(), Variable.GetDatum(), GetDatum(Value))); }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-set{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // variable assignment e.g. (define (x) ...) or (define x ...)
    // same as set! but does not require previous declaration
    public sealed class AstDefine : AST
    {
        private Syntax Keyword;              // set!                                     
        private Syntax Variable;             // x   
        private AST Value;                   // 99
        private Binding Binding;      //
        public AstDefine(Syntax syntax, Syntax keyword, Syntax variable, AST value, Binding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override Value GetDatum() { return new Value(ValueLinkedList.FromArguments(Expression.GetDatum(), Variable.GetDatum(), GetDatum(Value))); }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-def{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
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
            this.Keyword = keyword;
            this.condExpression = cond;
            this.thenExperssion = then;
            this.elseExpression = els;
        }
        public override Value GetDatum()
        {
            if (elseExpression == null)
                return new Value(ValueLinkedList.FromArguments(Keyword.GetDatum(), GetDatum(condExpression), GetDatum(thenExperssion)));
            else
                return new Value(ValueLinkedList.FromArguments(Keyword.GetDatum(), GetDatum(condExpression), GetDatum(thenExperssion), GetDatum(elseExpression)));
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
            this.Keyword = keyword;
            this.Conditions = conditions;
            this.ElseCase = elseCase;
        }
        public override Value GetDatum()
        {
            LinkedList<Value> resut = new LinkedList<Value>();
            resut.AddLast(Keyword.GetDatum());
            if (Conditions != null) resut.Append(GetDatum(Conditions).AsLinkedList<Value>());
            if (ElseCase != null) resut.AddLast(GetDatum(ElseCase));
            return new Value(resut);
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
            this.Identifier = identifier;
            this.Arguments = arguments;
        }
        public override Value GetDatum()
        {
            LinkedList<Value> resut = new LinkedList<Value>();
            resut.AddLast(Identifier.GetDatum());
            Value datum = GetDatum(Arguments);
            resut.Append(datum.AsLinkedList<Value>());
            return new Value(resut);
        }

        #region ValueType Methods
        public override string Inspect() { return string.Format("#<ast-prim{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // application e.g. (f 1 2)
    public sealed class AstApplication : AST
    {
        private LinkedList<Value> list;
        public AstApplication(Syntax syntax, LinkedList<Value> expression) : base(syntax)
        {
            this.list = expression;
        }
        public override Value GetDatum()
        {
            return GetDatum(list);
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
        private BaseArguments ArgList;               // (lambda <(...)> )
        private LinkedList<Value> BodyExpression;    // (lambda (...) <...>)
        public AstLambda(Syntax syntax, Syntax keyword, BaseArguments arguments, LinkedList<Value> expression) : base(syntax)
        {
            this.ArgList = arguments;
            this.BodyExpression = expression;
            if (keyword.GetDatum() == Symbol.LAMBDA)
                this.Keyword = keyword;
            else
                this.Keyword = new Syntax(Symbol.LAMBDA, keyword.Location);
        }
        public override Value GetDatum()
        {
            LinkedList<Value> list = new LinkedList<Value>();
            Value body = GetDatum(BodyExpression);

            list.AddLast(Keyword.GetDatum());
            list.AddLast(ArgList.AsDatum());

            list.Append(body.AsLinkedList<Value>());

            return new Value(list);
     
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
            this.Keyword = keyword;
            this.BodyExpression = expression;
        }
        public override Value GetDatum()
        {
            LinkedList<Value> resut = new LinkedList<Value>();
            resut.AddLast(Keyword.GetDatum());
            resut.Append(GetDatum(BodyExpression).AsLinkedList<Value>());
            return new Value(resut);
        }

        #region ValueType Methods
        public override string Inspect()
        {
            return string.Format("#<ast-seq{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

}