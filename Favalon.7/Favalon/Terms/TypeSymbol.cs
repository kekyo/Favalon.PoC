using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Terms
{
    public sealed class TypeSymbol : Symbol
    {
        public readonly TypeInfo Type;

        internal TypeSymbol(TypeInfo type) :
            base(Unspecified.Instance) =>   // TODO: Unspecified --> Kind
            this.Type = type;

        public override string PrintableName =>
            $"{this.Type.FullName}";

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public bool Equals(TypeSymbol? other) =>
            other?.Type.Equals(this.Type) ?? false;

        public override bool Equals(Symbol? other) =>
            this.Equals(other as TypeSymbol);

        protected internal override Expression Visit(Environment environment) =>
            throw new InvalidOperationException();
    }
}
