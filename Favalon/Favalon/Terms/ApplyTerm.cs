using System;

namespace Favalon.Terms
{
    public sealed class ApplyTerm :
        Term, IEquatable<ApplyTerm?>
    {
        public new readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override bool Reducible =>
            this.Function.Reducible || this.Argument.Reducible ||
            this.Function is FunctionTerm;

        public override Term VisitReplace(string identity, Term replacement) =>
            new ApplyTerm(
                this.Function.VisitReplace(identity, replacement),
                this.Argument.VisitReplace(identity, replacement));

        public override Term VisitReduce() =>
            this.Function.Reducible ?
                new ApplyTerm(this.Function.VisitReduce(), this.Argument) :
                this.Argument.Reducible ?
                    new ApplyTerm(this.Function, this.Argument.VisitReduce()) :
                    this.Function is FunctionTerm function ?
                        function.Call(this.Argument) :
                        this;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^
            this.Argument.GetHashCode();

        public bool Equals(ApplyTerm? other) =>
            (other?.Function.Equals(this.Function) ?? false) &&
            (other?.Argument.Equals(this.Argument) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as ApplyTerm);

        public override string ToString()
        {
            var function = this.Function is FunctionTerm ?
                $"({this.Function})" :
                this.Function.ToString();
            return this.Argument is IdentityTerm ?
                $"{function} {this.Argument}" :
                $"{function} ({this.Argument})";
        }
    }
}
