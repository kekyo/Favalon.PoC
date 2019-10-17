using Favalon.Expressions;
using System;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class MethodSymbol : Symbol
    {
        public readonly MethodBase Method;

        internal MethodSymbol(MethodBase method) :
            base(Unspecified.Instance) => // TODO: Kind
            this.Method = method;

        public override string PrintableName =>
            this.Method is ConstructorInfo ?
                $"{this.Method.DeclaringType.FullName}" :
                $"{this.Method.DeclaringType.FullName}.{this.Method.Name}";

        public override int GetHashCode() =>
            this.Method.GetHashCode();

        public bool Equals(MethodSymbol? other) =>
            other?.Method.Equals(this.Method) ?? false;

        public override bool Equals(Symbol? other) =>
            this.Equals(other as MethodSymbol);

        public override Expression VisitInfer(Environment environment) =>
            throw new InvalidOperationException();
    }
}
