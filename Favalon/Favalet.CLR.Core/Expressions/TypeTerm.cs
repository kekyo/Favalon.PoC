using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ITypeTerm : ITerm
    {
        Type RuntimeType { get; }
    }

    public sealed class TypeTerm :
        Expression, ITypeTerm
    {
        public readonly Type RuntimeType;

        private TypeTerm(Type runtimeType) =>
            this.RuntimeType = runtimeType;

        public IExpression HigherOrder =>
            Generator.Kind();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Type ITypeTerm.RuntimeType =>
            this.RuntimeType;

        public override int GetHashCode() =>
            this.RuntimeType.GetHashCode();

        public bool Equals(ITypeTerm rhs) =>
            this.RuntimeType.Equals(rhs.RuntimeType);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ITypeTerm rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => this.RuntimeType.FullName,
                _ => $"(Type {this.RuntimeType.FullName})"
            };

        public static ITerm From(Type type)
        {
            if (type.Equals(typeof(object).GetType()))
            {
                return Generator.Kind();
            }
            else
            {
                return new TypeTerm(type);
            }
        }
    }

    public static class TypeTermExtension
    {
        public static void Deconstruct(
            this ITypeTerm type,
            out Type runtimeType) =>
            runtimeType = type.RuntimeType;
    }
}
