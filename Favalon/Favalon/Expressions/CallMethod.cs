using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class CallMethod : Expression
    {
        public readonly MethodInfo Method;
        public readonly Expression Argument;

        internal CallMethod(MethodInfo method, Expression argument)
        {
            this.Method = method;
            this.Argument = argument;
            this.HigherOrder = Factories.FromType((this.Method.ReturnType.GetTypeInfo()));
        }

        public override Expression HigherOrder { get; }

        public override int GetHashCode() =>
            this.Method.GetHashCode();

        public bool Equals(CallMethod? other) =>
            (other?.Method.Equals(this.Method) ?? false) &&
            (other?.Argument.Equals(this.Argument) ?? false);

        public override bool Equals(Expression? other) =>
            this.Equals(other as CallMethod);

        public override string ToString() =>
            $"{this.Method.DeclaringType.FullName}.{this.Method.Name} {this.Argument}";

        public override Expression Run() =>
            Factories.Value(this.Method.Invoke(null, new[] { ((Instance)this.Argument.Run()).Value }));
    }
}
