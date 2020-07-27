using Favalet.Contexts;
using Favalet.Internal;
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

        public override IExpression HigherOrder =>
            Generator.Kind();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Type ITypeTerm.RuntimeType =>
            this.RuntimeType;

        public override int GetHashCode() =>
            this.RuntimeType.GetHashCode();

        public bool Equals(ITypeTerm rhs) =>
            this.RuntimeType.Equals(rhs.RuntimeType);

        public override bool Equals(IExpression? other) =>
            other is ITypeTerm rhs && Equals(rhs);

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    this.RuntimeType.GetReadableName() :
                    $"Type {this.RuntimeType.GetReadableName()}");

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
