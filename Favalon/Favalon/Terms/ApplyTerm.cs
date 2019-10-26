namespace Favalon.Terms
{
    public sealed class ApplyTerm : Term
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

        public override Term VisitReplace(string name, Term replacement) =>
            new ApplyTerm(
                this.Function.VisitReplace(name, replacement),
                this.Argument.VisitReplace(name, replacement));

        public override Term VisitReduce() =>
            this.Function.Reducible ?
                new ApplyTerm(this.Function.VisitReduce(), this.Argument) :
                this.Argument.Reducible ?
                    new ApplyTerm(this.Function, this.Argument.VisitReduce()) :
                    this.Function is FunctionTerm function ?
                        function.Call(this.Argument) :
                        this;

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
