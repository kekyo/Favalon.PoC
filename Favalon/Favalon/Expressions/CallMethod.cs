using System.Reflection;

namespace Favalon.Expressions
{
    public sealed class CallMethod : Expression
    {
        public readonly MethodBase Method;
        public readonly Expression Argument;

        internal CallMethod(MethodBase method, Expression argument)
        {
            this.Method = method;
            this.Argument = argument;

            var returnType = method is ConstructorInfo ci ?
                ci.DeclaringType :
                ((MethodInfo)method).ReturnType;
            this.HigherOrder = Factories.FromType(returnType);
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
            this.Method is ConstructorInfo ?
                $"{this.Method.DeclaringType.FullName} {this.Argument}" :
                $"{this.Method.DeclaringType.FullName}.{this.Method.Name} {this.Argument}";

        public override Expression Run() =>
            Factories.Value(this.Method.Invoke(null, new[] { ((Value)this.Argument.Run()).RawValue }));
    }
}
