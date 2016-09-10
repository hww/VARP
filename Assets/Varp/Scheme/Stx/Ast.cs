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
    public class AST : SObject
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
        public virtual SObject GetDatum() { return GetDatum(this); }
        public T GetDatum<T>() where T:class { return GetDatum(this) as T; }
        public SObject GetDatum(SObject obj)
        {
            if (obj == null) return null;
            if (obj is Syntax) return (obj as Syntax).GetDatum();
            if (obj is AST) return (obj as AST).GetDatum();
            if (obj is Pair)
            {
                Pair curent = obj as Pair;
                Pair first = new Pair();
                Pair last = first;
                while (curent != null)
                {
                    last.Car = GetDatum(curent.Car);
                    if (curent.Cdr is Pair)
                    {
                        curent = curent.Cdr as Pair;
                        last.Cdr = new Pair();
                        last = last.Cdr as Pair;
                    }
                    else
                    {
                        // . syntax
                        last.Cdr = GetDatum(curent.Cdr);
                        curent = null;
                        break;
                    }
                }
                return first;
            }
            return obj;
        }
        #region SObject Methods
        public override SBool AsBool() { return SBool.True; }
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
        public override SObject GetDatum() { return GetDatum(Expression); }

        #region SObject Methods
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
        public LexicalBinding Binding;
        public AstReference(Syntax syntax, LexicalBinding binding) : base(syntax)
        {
            Binding = binding;
        }
        public override SObject GetDatum() { return GetDatum(Expression); }

        #region SObject Methods
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
        public LexicalBinding Binding;      //
        public AstSet(Syntax syntax, Syntax keyword, Syntax variable, AST value, LexicalBinding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override SObject GetDatum() { return Pair.ListFromArguments(Expression.GetDatum(), Variable.GetDatum(), GetDatum(Value)); }

        #region SObject Methods
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
        public LexicalBinding Binding;      //
        public AstDefine(Syntax syntax, Syntax keyword, Syntax variable, AST value, LexicalBinding binding) : base(syntax)
        {
            this.Keyword = keyword;
            this.Variable = variable;
            this.Value = value;
            this.Binding = binding;
        }
        public override SObject GetDatum() { return Pair.ListFromArguments(Expression.GetDatum(), Variable.GetDatum(), GetDatum(Value)); }

        #region SObject Methods
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
        public override SObject GetDatum()
        {
            if (elseExpression == null)
                return Pair.ListFromArguments(Keyword.GetDatum(), GetDatum(condExpression), GetDatum(thenExperssion));
            else
                return Pair.ListFromArguments(Keyword.GetDatum(), GetDatum(condExpression), GetDatum(thenExperssion), GetDatum(elseExpression));
        }
        #region SObject Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-if{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // conditional e.g. (cond (() .. ) (() ...) (else ...))
    public sealed class AstConditionCond : AST
    {
        public Syntax Keyword;
        public Pair Conditions;     //< list of pairs
        public Pair ElseCase;       //< else condition

        public AstConditionCond(Syntax syntax, Syntax keyword, Pair conditions, Pair elseCase) : base(syntax)
        {
            this.Keyword = keyword;
            this.Conditions = conditions;
            this.ElseCase = elseCase;
        }
        public override SObject GetDatum()
        {

            Pair conditions = GetDatum(Conditions) as Pair;
            if (ElseCase != null)
            {
                Pair else_case = new Pair(GetDatum(ElseCase), null);
                conditions = Pair.Append(conditions, else_case) as Pair;
            }
            return new Pair(Keyword.GetDatum(), conditions);

        }

        #region SObject Methods
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
        public Pair Arguments;
        public AstPrimitive(Syntax syntax, Syntax identifier, Pair arguments) : base(syntax)
        {
            this.Identifier = identifier;
            this.Arguments = arguments;
        }
        public override SObject GetDatum()
        {
            return new Pair(Identifier.GetDatum(), GetDatum(Arguments));
        }

        #region SObject Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect() { return string.Format("#<ast-prim{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum())); }
        #endregion
    }

    // application e.g. (f 1 2)
    public sealed class AstApplication : AST
    {
        Pair expression;
        public AstApplication(Syntax syntax, Pair expression) : base(syntax)
        {
            this.expression = expression;
        }
        public override SObject GetDatum()
        {
            return GetDatum(expression);
        }
        #region SObject Methods
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
        public Syntax Keyword;       // (<lambda> (...) ...)
        public Arguments ArgList;  // (lambda <(...)> )
        public Pair BodyExpression;  // (lambda (...) <...>)
        public AstLambda(Syntax syntax, Syntax keyword, Arguments arguments, Pair expression) : base(syntax)
        {
            this.ArgList = arguments;
            this.BodyExpression = expression;
            if (keyword.GetDatum() == Symbol.LAMBDA)
                this.Keyword = keyword;
            else
                this.Keyword = new Syntax(Symbol.LAMBDA, keyword.location);
        }
        public override SObject GetDatum()
        {

            Pair args = null;
            Pair last = null;

            if (ArgList != null)
            {
                Pair req = ArgList.required;
                Pair opt = ArgList.optional;
                Pair key = ArgList.key;
                Pair rst = ArgList.rest;

                Pair.Duplicate(req, ref args, ref last);

                if (opt != null)
                {
                    Pair.Duplicate(opt, ref last.Cdr, ref last);
                }
                if (key != null)
                {
                    Pair.Duplicate(key, ref last.Cdr, ref last);
                }
                if (rst != null)
                {
                    switch (rst.Count)
                    {
                        case 0:
                            break;
                        case 1:
                            if (true)
                                throw new SyntaxError(Expression, "lambda: too many &rest arguments: " + Inspector.Inspect(rst));
                            else
                                last.Cdr = rst.Car;
                            break;
                        case 2:
                            Pair.Duplicate(rst, ref last.Cdr, ref last);
                            break;
                        default:
                            throw new SyntaxError(Expression, "lambda: too many &rest arguments: " + Inspector.Inspect(rst));
                    }
                }
            }
            Pair list = Pair.ListFromArguments(ref last, Keyword.GetDatum(), GetDatum(args));
            last.Cdr = GetDatum(BodyExpression);
            return list;

        }

        #region SObject Methods
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
        public Pair BodyExpression;
        public AstSequence(Syntax syntax, Syntax keyword, Pair expression) : base(syntax)
        {
            this.Keyword = keyword;
            this.BodyExpression = expression;
        }
        public override SObject GetDatum()
        {
            return new Pair(Keyword.GetDatum(), GetDatum(BodyExpression));
        }

        #region SObject Methods
        public override string AsString() { return base.ToString(); }
        public override string Inspect()
        {
            return string.Format("#<ast-seq{0} {1}>", GetLocationString(), Inspector.Inspect(GetDatum()));
        }
        #endregion
    }

}