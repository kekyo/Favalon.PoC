using Favalon.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class MethodTerm :
        CallableTerm, IEquatable<MethodTerm>
    {
        public new readonly MethodInfo Method;

        internal MethodTerm(MethodInfo method) =>
            this.Method = method;

        public override IdentityTerm Parameter =>
            new IdentityTerm(this.Method.GetParameters().Single().Name);

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitCall(Context context, Term argument) =>
            new ConstantTerm(
                this.Method.Invoke(
                    null,
                    new object[] { ((ConstantTerm)context.Reduce(argument)).Constant }));

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^
            this.Method.GetHashCode();

        public bool Equals(MethodTerm? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Method.Equals(this.Method) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as MethodTerm);

        protected override string VisitTermString(bool includeTermName) =>
            $"{this.Method.GetFullName()}({this.Parameter.ToString(includeTermName)})";

        public void Deconstruct(out Term parameter, out MethodInfo method)
        {
            parameter = this.Parameter;
            method = this.Method;
        }
    }
}
