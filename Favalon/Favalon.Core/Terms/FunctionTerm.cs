using System;

namespace Favalon.Terms
{
    public sealed class FunctionTerm :
        CallableTerm, IEquatable<FunctionTerm>
    {
        public readonly Term Body;

        internal FunctionTerm(BoundIdentityTerm parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override BoundIdentityTerm Parameter { get; }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            (this.Parameter.Name == identity) ?
                this :  // NOT applicable
                new FunctionTerm(
                    (BoundIdentityTerm)this.Parameter.VisitReplace(identity, replacement),
                    this.Body.VisitReplace(identity, replacement));

        protected internal override Term VisitCall(Context context, Term argument) =>
            this.Body.VisitReplace(
                this.Parameter.Name,
                argument);

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^
            this.Body.GetHashCode();

        public bool Equals(FunctionTerm? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Body.Equals(this.Body) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as FunctionTerm);

        protected internal override string VisitTermString(bool includeTermName) =>
            $"{this.Parameter.ToString(includeTermName)} -> {this.Body.ToString(includeTermName)}";

        public void Deconstruct(out BoundIdentityTerm parameter, out Term body)
        {
            parameter = this.Parameter;
            body = this.Body;
        }
    }
}
