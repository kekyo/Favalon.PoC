using System;

namespace LambdaCalculus
{
    public abstract class Term
    {
        public abstract Term Reduce();

        public abstract Term Replace(string parameter, Term replacement);
    }

    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        public BooleanTerm(bool value) =>
            this.Value = value;

        public override Term Reduce() =>
            this;

        public override Term Replace(string parameter, Term replacement) =>
            this;
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

        public override Term Reduce() =>
            ((CallableTerm)this.Function.Reduce()).Call(this.Argument.Reduce());

        public override Term Replace(string parameter, Term replacement) =>
            new ApplyTerm(this.Function.Replace(parameter, replacement), this.Argument.Replace(parameter, replacement));
    }

    public abstract class CallableTerm : Term
    {
        public abstract Term Call(Term rhs);
    }

    public sealed class AndTerm : CallableTerm
    {
        public readonly Term Lhs;

        public AndTerm(Term lhs) =>
            this.Lhs = lhs;

        public override Term Reduce() =>
            new AndTerm(this.Lhs.Reduce());

        public override Term Call(Term rhs) =>
            new BooleanTerm(((BooleanTerm)this.Lhs).Value && ((BooleanTerm)rhs).Value);

        public override Term Replace(string parameter, Term replacement) =>
            new AndTerm(this.Lhs.Replace(parameter, replacement));
    }

    public sealed class LambdaTerm : CallableTerm
    {
        public readonly string Parameter;
        public readonly Term Body;

        public LambdaTerm(string parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term Reduce() =>
            this;

        public override Term Call(Term rhs) =>
            this.Body.Replace(this.Parameter, rhs).Reduce();

        public override Term Replace(string parameter, Term replacement) =>
            (parameter != this.Parameter) ?
                this.Body.Replace(parameter, replacement) :
                this;   // Shadowed
    }
}
