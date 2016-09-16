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

namespace VARP.Scheme.Stx
{
    using Data;
    using Exception;
    using REPL;

    /// <summary>
    /// This structure have to be used as shortcuts to already existing type such as
    /// Syntax and Pair. Use the data in existing data structure and do not make copy
    /// values without reason. It will make more loaded GC but at same time simpify code.
    /// </summary>
    public class AST : ValueClass
    {
        /// <summary>
        /// This is location of the expression
        /// For instance the expression (+ 1 2) will have
        /// position at first open bracket
        /// But for literal x the position of expression is
        /// position of this literal
        /// </summary>
        public Syntax Expression;   //< for expression (+ 1 2) will be "("

        public AST(Syntax syntax)
        {
            this.Expression = syntax;
        }

        // n.b. will be reimplemented for most of types.
        public virtual Value GetDatum() { return Value.Nill; }
        public T GetDatum<T>() where T:class { return GetDatum(this) as T; }
        public Value GetDatum(AST ast)
        {
            return ast.GetDatum();
        }
        public Value GetDatum(Syntax syn)
        {
            return syn.AsDatum();
        }
        public Value GetDatum(Value value)
        {
            if (value == null) return Value.Nill;
            if (value.IsSyntax)
                return value.AsSyntax().AsDatum();
            if (value.IsAST)
                return value.AsAST().GetDatum();
            if (value.IsValueList)
                return GetDatum(value.AsValueList());
            if (value.IsValueVector)
                return GetDatum(value.AsValueVector());
            return new Value(value);
        }
        public Value GetDatum(ValueList list)
        {
            ValueList result = new ValueList();
            foreach (var curent in list)
                result.AddLast(curent.AsAST().GetDatum());
            return new Value(result);
        }
        public Value GetDatum(ValueVector vector)
        {
            ValueVector result = new ValueVector(vector.Count);
            foreach (var curent in vector)
                result.Add(curent.AsAST().GetDatum());
            return new Value(result);
        }

        #region ValueType Methods
        public override bool AsBool() { return true; }
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast{0}>", GetLocationString()); }
        #endregion
        protected string GetLocationString()
        {
            if (Expression == null) return string.Empty;
            if (Expression.location == null) return string.Empty;
            return string.Format(":{0}:{1}", Expression.location.LineNumber, Expression.location.ColNumber);
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
        public override string AsString() { return Expression.AsString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-lit{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum(Expression)));
        }
        #endregion
    }

    // variable reference  e.g. x
    public sealed class AstReference : AST
    {
        public Binding Binding;
        public AstReference(Syntax syntax, Binding binding) : base(syntax)
        {
            Binding = binding;
        }
        public override Value GetDatum() { return GetDatum(Expression); }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-ref{0} {1}>", GetLocationString(), Binding.Identifier); }
        #endregion
    }

    // variable assignment e.g. (set! x 99)
    public sealed class AstSet : AST
    {
        public Syntax Keyword;              // set!                                     
        public Syntax Variable;             // x   
        public AST Value;                   // 99
        public Binding Binding;      //
        public AstSet(Syntax syntax, Syntax keyword, Syntax variable, AST value, Binding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override Value GetDatum() { return ValueList.ListFromArguments(Expression.AsDatum(), Variable.AsDatum(), GetDatum(Value)).ToValue(); }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-set{0} {1} {2}>", GetLocationString(), Variable.AsString(), Inspector.Inspect(Value)); }
        #endregion
    }

    // variable assignment e.g. (define (x) ...) or (define x ...)
    // same as set! but does not require previous declaration
    public sealed class AstDefine : AST
    {
        public Syntax Keyword;              // set!                                     
        public Syntax Variable;             // x   
        public AST Value;                   // 99
        public Binding Binding;      //
        public AstDefine(Syntax syntax, Syntax keyword, Syntax variable, AST value, Binding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override Value GetDatum() { return ValueList.ListFromArguments(Expression.AsDatum(), Variable.AsDatum(), GetDatum(Value)).ToValue(); }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-def{0} {1} {2}>", GetLocationString(), Variable.AsString(), Inspector.Inspect(GetDatum(Value))); }
        #endregion
    }

    // conditional e.g. (if 1 2 3)
    public sealed class AstIfCondition : AST
    {
        public Syntax Keyword;
        public AST condExpression;        // 1
        public AST thenExperssion;        // 2
        public AST elseExpression;        // 3

        public AstIfCondition(Syntax syntax, Syntax keyword, AST cond, AST then, AST els) : base(syntax)
        {
            this.Keyword = keyword;
            this.condExpression = cond;
            this.thenExperssion = then;
            this.elseExpression = els;
        }
        public override Value GetDatum()
        {
            if (elseExpression == null)
                return ValueList.ListFromArguments(Keyword.AsDatum(), GetDatum(condExpression), GetDatum(thenExperssion)).ToValue();
            else
                return ValueList.ListFromArguments(Keyword.AsDatum(), GetDatum(condExpression), GetDatum(thenExperssion), GetDatum(elseExpression)).ToValue();
        }
        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-if{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // conditional e.g. (cond (() .. ) (() ...) (else ...))
    public sealed class AstConditionCond : AST
    {
        public Syntax Keyword;
        public ValueList Conditions;     //< list of pairs
        public ValueList ElseCase;       //< else condition

        public AstConditionCond(Syntax syntax, Syntax keyword, ValueList conditions, ValueList elseCase) : base(syntax)
        {
            this.Keyword = keyword;
            this.Conditions = conditions;
            this.ElseCase = elseCase;
        }
        public override Value GetDatum()
        {
            ValueList resut = new ValueList();
            resut.AddLast(Keyword.AsDatum());
            resut.Append(GetDatum(Conditions).AsValueList());
            resut.Append(GetDatum(ElseCase).AsValueList());
            return resut.ToValue();
        }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-if{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

    // primitive op e.g. (+ 1 2)
    public sealed class AstPrimitive : AST
    {
        public Syntax Identifier;
        public ValueList Arguments;
        public AstPrimitive(Syntax syntax, Syntax identifier, ValueList arguments) : base(syntax)
        {
            this.Identifier = identifier;
            this.Arguments = arguments;
        }
        public override Value GetDatum()
        {
            ValueList resut = new ValueList();
            resut.AddLast(Identifier.AsDatum());
            resut.Append(GetDatum(Arguments).AsValueList());
            return resut.ToValue();
        }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-prim{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // application e.g. (f 1 2)
    public sealed class AstApplication : AST
    {
        ValueList expression;
        public AstApplication(Syntax syntax, ValueList expression) : base(syntax)
        {
            this.expression = expression;
        }
        public override Value GetDatum()
        {
            return GetDatum(expression);
        }
        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-app{0} {1}>", GetLocationString(),
                Inspector.Inspect(expression));
        }
        #endregion
    }

    // lambda expression   e.g. (lambda(x) x)
    public sealed class AstLambda : AST
    {
        public Syntax Keyword;              // (<lambda> (...) ...)
        public Arguments ArgList;           // (lambda <(...)> )
        public ValueList BodyExpression;    // (lambda (...) <...>)
        public AstLambda(Syntax syntax, Syntax keyword, Arguments arguments, ValueList expression) : base(syntax)
        {
            this.ArgList = arguments;
            this.BodyExpression = expression;
            if (keyword.AsDatum() == Symbol.LAMBDA)
                this.Keyword = keyword;
            else
                this.Keyword = new Syntax(Symbol.LAMBDA, keyword.location);
        }
        public override Value GetDatum()
        {
            ValueList list = new ValueList();

            list.AddLast(Keyword.AsDatum());
            list.AddLast(ArgList.AsDatum());
            list.AddLast(GetDatum(BodyExpression));

            return list.ToValue();
     
        }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-lam{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

    // sequence e.g. (begin 1 2)
    public sealed class AstSequence : AST
    {
        public Syntax Keyword;
        public ValueList BodyExpression;
        public AstSequence(Syntax syntax, Syntax keyword, ValueList expression) : base(syntax)
        {
            this.Keyword = keyword;
            this.BodyExpression = expression;
        }
        public override Value GetDatum()
        {
            ValueList resut = new ValueList();
            resut.AddLast(Keyword.AsDatum());
            resut.Append(GetDatum(BodyExpression).AsValueList());
            return resut.ToValue();
        }

        #region ValueType Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-seq{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

}