using Favalon.Internal;
using System;

namespace Favalon.Terms
{
    public sealed class DelegationTerm<TParameterTerm> :
        CallableTerm, IEquatable<DelegationTerm<TParameterTerm>>
        where TParameterTerm : Term
    {
        public new readonly IdentityTerm Identity;

        private readonly Func<Context, TParameterTerm, Term> interpreter;

        internal DelegationTerm(string identity, string parameter,
            Func<Context, TParameterTerm, Term> interpreter)
        {
            this.Identity = new IdentityTerm(identity);
            this.Parameter = new BoundIdentityTerm(parameter/* TODO: , new ClrTypeTerm(typeof(TParameterType)) */);
            this.interpreter = interpreter;
        }

        public override BoundIdentityTerm Parameter { get; }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitCall(Context context, Term argument) =>
            // Will cause failure when argument isn't TParameterTerm
            interpreter(context, (TParameterTerm)argument);

        public override int GetHashCode() =>
            this.Identity.GetHashCode() ^
            this.Parameter.GetHashCode();

        public bool Equals(DelegationTerm<TParameterTerm>? other) =>
            other?.Identity.Equals(this.Identity) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as DelegationTerm<TParameterTerm>);

        protected internal override string VisitTermString(bool includeTermName) =>
            $"{this.Identity.ToString(includeTermName)} {this.Parameter.ToString(includeTermName)} {this.interpreter.GetIdentity()}";

        public void Deconstruct(out string identity) =>
            identity = this.Identity.Name;
    }
}
