using System;

namespace Favalon.Terms
{
    public sealed class InterpretTerm :
        CallableTerm, IEquatable<InterpretTerm>
    {
        public new readonly IdentityTerm Identity;

        private readonly Func<Context, Term, Term> interpreter;

        internal InterpretTerm(string identity, string parameter,
            Func<Context, Term, Term> interpreter)
        {
            this.Identity = new IdentityTerm(identity);
            this.Parameter = new IdentityTerm(parameter);
            this.interpreter = interpreter;
        }

        public override IdentityTerm Parameter { get; }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitCall(Context context, Term argument) =>
            interpreter(context, argument);

        public override int GetHashCode() =>
            this.Identity.GetHashCode() ^
            this.Parameter.GetHashCode();

        public bool Equals(InterpretTerm? other) =>
            other?.Identity.Equals(this.Identity) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as InterpretTerm);

        protected override string VisitTermString(bool includeTermName) =>
            $"{this.Identity.ToString(includeTermName)} {this.Parameter.ToString(includeTermName)}";

        public void Deconstruct(out string identity) =>
            identity = this.Identity.Name;
    }
}
