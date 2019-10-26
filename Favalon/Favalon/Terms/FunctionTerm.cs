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

        public override Term VisitReplace(string identity, Term replacement) =>
            (this.Parameter is IdentityTerm variable && variable.Identity == identity) ?
                this :  // NOT applicable
                new FunctionTerm(
                    this.Parameter.VisitReplace(identity, replacement),
                    this.Body.VisitReplace(identity, replacement));

        public override Term VisitReduce() =>
            this;

        public Term Call(Term argument) =>
            this.Body.VisitReplace(
                ((IdentityTerm)this.Parameter).Identity,
                argument);

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^
            this.Body.GetHashCode();

        public bool Equals(FunctionTerm? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Body.Equals(this.Body) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as FunctionTerm);

        public override string ToString()
        {
            var parameter = (this.Parameter is FunctionTerm) ?
                $"({this.Parameter})" : this.Parameter.ToString();
            return $"{parameter} -> {this.Body}";
        }
    }
}
