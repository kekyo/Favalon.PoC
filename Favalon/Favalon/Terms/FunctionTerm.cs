using System;

namespace Favalon.Terms
{
    public sealed class FunctionTerm : Term
    {
        public readonly Term Parameter;
        public readonly Term Body;

        internal FunctionTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override bool Reducible =>
            false;

        public override Term VisitReplace(string name, Term replacement) =>
            (this.Parameter is IdentityTerm variable && variable.Name == name) ?
                this :  // NOT applicable
                new FunctionTerm(
                    this.Parameter.VisitReplace(name, replacement),
                    this.Body.VisitReplace(name, replacement));

        public override Term VisitReduce() =>
            this;

        public Term Call(Term argument) =>
            this.Body.VisitReplace(
                ((IdentityTerm)this.Parameter).Name,
                argument);

        public override string ToString()
        {
            var parameter = (this.Parameter is FunctionTerm) ?
                $"({this.Parameter})" : this.Parameter.ToString();
            return $"{parameter} -> {this.Body}";
        }
    }
}
