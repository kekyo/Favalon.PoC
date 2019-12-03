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

    ////////////////////////////////////////////////////////////

    public abstract class Term :
        IEquatable<Term?>
    {
        public abstract Term HigherOrder { get; }

        public abstract Term Reduce(Context context);

        public abstract Term Infer(Context context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override bool Equals(object? other) =>
            this.Equals(other as Term);

        //////////////////////////////////

        private static readonly Dictionary<Type, IdentityTerm> types =
            new Dictionary<Type, IdentityTerm>();

        private static readonly IdentityTerm kind =
            new IdentityTerm("*", UnspecifiedTerm.Instance);

        public static IdentityTerm Identity(Type type)
        {
            if (!types.TryGetValue(type, out var term))
            {
                term = new IdentityTerm(type.FullName, kind);
                types.Add(type, term);
            }
            return term;
        }

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        public static IdentityTerm Kind() =>
            kind;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static BooleanTerm Constant(bool value) =>
            value ? BooleanTerm.True : BooleanTerm.False;

        public static Term Constant(object value) =>
            new ConstantTerm(value);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            new LambdaTerm(parameter, body);

        public static AndTerm And(Term lhs, Term rhs) =>
            new AndTerm(lhs, rhs);

        public static Term If(Term condition, Term then, Term els) =>
            new IfTerm(condition, then, els);
    }

    ////////////////////////////////////////////////////////////

    public sealed class UnspecifiedTerm : Term
    {
        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Reduce(Context context) =>
            this;

        public override Term Infer(Context context) =>
            this;

        public override bool Equals(Term? other) =>
            other is UnspecifiedTerm;

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    public sealed class IdentityTerm : Term
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Reduce(context) :
                this;

        public override Term Infer(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Infer(context) :
                this;

        public override bool Equals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;
    }

    public sealed class ConstantTerm : Term
    {
        public readonly object Value;

        internal ConstantTerm(object value) =>
            this.Value = value;

        public override Term HigherOrder =>
            Constant(this.Value.GetType());

        public override Term Reduce(Context context) =>
            this;

        public override Term Infer(Context context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ConstantTerm rhs ? this.Value.Equals(rhs.Value) : false;
    }

    public abstract class ApplicableTerm : Term
    {
        protected internal abstract Term? Apply(Context context, Term rhs);
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term HigherOrder =>
            this.Argument.HigherOrder;

        public override Term Reduce(Context context)
        {
            var function = this.Function.Reduce(context);

            if (function is ApplicableTerm applicable &&
                applicable.Apply(context, this.Argument) is Term term)
            {
                return term;
            }
            else
            {
                return new ApplyTerm(function, this.Argument.Reduce(context));
            }
        }

        public override Term Infer(Context context)
        {
            var function = this.Function.Infer(context);
            var argument = this.Argument.Infer(context);

            return new ApplyTerm(function, argument);
        }

        public override bool Equals(Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(rhs.Function) && this.Argument.Equals(rhs.Argument)) :
                false;
    }
}
