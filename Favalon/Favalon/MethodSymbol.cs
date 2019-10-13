using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon
{
    public sealed class MethodSymbol : Symbol
    {
        public readonly MethodInfo Method;

        internal MethodSymbol(MethodInfo method) :
            base(Unspecified.Instance) => // TODO: Kind
            this.Method = method;

        public override string PrintableName =>
            $"{this.Method.DeclaringType.FullName}.{this.Method.Name}";

        public override int GetHashCode() =>
            this.Method.GetHashCode();

        public bool Equals(MethodSymbol? other) =>
            other?.Method.Equals(this.Method) ?? false;

        public override bool Equals(Symbol? other) =>
            this.Equals(other as MethodSymbol);

        public override Term VisitInfer(Environment environment) =>
            this;
    }
}
