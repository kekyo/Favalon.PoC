using Favalon.Internal;
using System;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class DelegationTerm<TParameterTerm> :
        CallableTerm, IEquatable<DelegationTerm<TParameterTerm>>
        where TParameterTerm : Term
    {
        private readonly Func<Context, TParameterTerm, Term> runner;

        internal DelegationTerm(string parameter,
            Func<Context, TParameterTerm, Term> runner)
        {
            this.Parameter = new BoundIdentityTerm(parameter/* TODO: , new ClrTypeTerm(typeof(TParameterType)) */);
            this.runner = runner;
        }

        public override BoundIdentityTerm Parameter { get; }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitCall(Context context, Term argument) =>
            // Will cause failure when argument isn't TParameterTerm
            runner(context, (TParameterTerm)argument);

        public override int GetHashCode() =>
            this.runner.GetMethodInfo().GetHashCode();

        public bool Equals(DelegationTerm<TParameterTerm>? other) =>
            (other?.runner.GetMethodInfo().Equals(this.runner.GetMethodInfo()) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as DelegationTerm<TParameterTerm>);

        protected internal override string VisitTermString(bool includeTermName) =>
            $"{this.runner.GetIdentity()} {this.Parameter.ToString(includeTermName)}";
    }
}
