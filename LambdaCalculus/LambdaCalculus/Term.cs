using System;
using System.Collections.Generic;

namespace LambdaCalculus
{
    public sealed class Context
    {
        private readonly Context? parent;
        private Dictionary<string, Term>? boundTerms;

        public Context() =>
            boundTerms = new Dictionary<string, Term>();
        private Context(Context parent) =>
            this.parent = parent;

        public Context NewScope() =>
            new Context(this);

        public void AddBoundTerm(string identity, Term term)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }
            boundTerms[identity] = term;
        }

        public Term? GetBoundTerm(string identity)
        {
            Context? current = this;
            do
            {
                if (current.boundTerms != null)
                {
                    if (current.boundTerms.TryGetValue(identity, out var term))
                    {
                        return term;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            return null;
        }
    }

    public abstract class Term
    {
        public abstract Term Reduce(Context context);
    }

    public sealed class IdentityTerm : Term
    {
        public readonly string Identity;

        public IdentityTerm(string identity) =>
            this.Identity = identity;

        public override Term Reduce(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term :
                this;
    }

    public abstract class ApplicableTerm : Term
    {
        public abstract Term Apply(Context context, Term rhs);
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        public ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term Reduce(Context context)
        {
            var function = this.Function.Reduce(context);
            var argument = this.Argument.Reduce(context);

            if (function is ApplicableTerm applicable)
            {
                return applicable.Apply(context, argument);
            }
            else
            {
                return new ApplyTerm(function, argument);
            }
        }
    }

    public sealed class LambdaTerm : ApplicableTerm
    {
        public readonly string Parameter;
        public readonly Term Body;

        public LambdaTerm(string parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term Reduce(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Reduce(context));

        public override Term Apply(Context context, Term rhs)
        {
            var newScope = context.NewScope();

            newScope.AddBoundTerm(this.Parameter, rhs);
            return this.Body.Reduce(newScope);
        }
    }

    ////////////////////////////////////////////////////////////

    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        public BooleanTerm(bool value) =>
            this.Value = value;

        public override Term Reduce(Context context) =>
            this;
    }

    public sealed class AndTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        public AndTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public override Term Reduce(Context context)
        {
            var lhs = this.Lhs.Reduce(context);
            var rhs = this.Rhs.Reduce(context);
            if (lhs is BooleanTerm l && rhs is BooleanTerm r)
            {
                return new BooleanTerm(l.Value && r.Value);
            }
            else
            {
                return new AndTerm(lhs, rhs);
            }
        }
    }
}
