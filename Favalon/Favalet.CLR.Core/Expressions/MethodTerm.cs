﻿using Favalet.Contexts;
using Favalet.Internal;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Favalet.Expressions
{
    public interface IMethodTerm :
        ITerm, ICallableExpression
    {
        MethodBase RuntimeMethod { get; }
    }

    public sealed class MethodTerm :
        Expression, IMethodTerm
    {
        public readonly MethodBase RuntimeMethod;

        private MethodTerm(MethodBase runtimeMethod) =>
            this.RuntimeMethod = runtimeMethod;

        public override IExpression HigherOrder =>
            FunctionExpression.Create(
                TypeTerm.From(this.RuntimeMethod.GetParameters()[0].ParameterType),
                TypeTerm.From(
                    this.RuntimeMethod is MethodInfo method ?
                        method.ReturnType :
                        this.RuntimeMethod.DeclaringType));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        MethodBase IMethodTerm.RuntimeMethod =>
            this.RuntimeMethod;

        public override int GetHashCode() =>
            this.RuntimeMethod.GetHashCode();

        public bool Equals(IMethodTerm rhs) =>
            this.RuntimeMethod.Equals(rhs.RuntimeMethod);

        public override bool Equals(IExpression? other) =>
            other is IMethodTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public IExpression Call(IReduceContext context, IExpression argument)
        {
            if (argument is IConstantTerm constant)
            {
                if (this.RuntimeMethod is ConstructorInfo constructor)
                {
                    var result = constructor.Invoke(new[] { constant.Value });
                    return ConstantTerm.From(result);
                }
                else
                {
                    var method = (MethodInfo)this.RuntimeMethod;
                    var result = method.Invoke(null, new[] { constant.Value });
                    return ConstantTerm.From(result);
                }
            }
            else
            {
                throw new ArgumentException(argument.GetPrettyString(PrettyStringContext.Simple));
            }
        }

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    this.RuntimeMethod.GetReadableName() :
                    $"Method {this.RuntimeMethod.GetReadableName()}");

        public static MethodTerm From(MethodBase method) =>
            new MethodTerm(method);
    }

    public static class MethodTermExtension
    {
        public static void Deconstruct(
            this IMethodTerm method,
            out MethodBase runtimeMethod) =>
            runtimeMethod = method.RuntimeMethod;
    }
}
