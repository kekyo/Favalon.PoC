using Favalet.Contexts;
using Favalet.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

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
        private readonly LazySlim<FunctionExpression> higherOrder;

        public readonly MethodBase RuntimeMethod;

        [DebuggerStepThrough]
        private MethodTerm(MethodBase runtimeMethod)
        {
            this.RuntimeMethod = runtimeMethod;
            this.higherOrder = LazySlim.Create(() =>
                FunctionExpression.Create(
                    TypeTerm.From(this.RuntimeMethod.GetParameters()[0].ParameterType),
                    TypeTerm.From(
                        this.RuntimeMethod is MethodInfo method ?
                            method.ReturnType :
                            this.RuntimeMethod.DeclaringType)));
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get => this.higherOrder.Value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        MethodBase IMethodTerm.RuntimeMethod
        {
            [DebuggerStepThrough]
            get => this.RuntimeMethod;
        }

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
                throw new ArgumentException(argument.GetPrettyString(PrettyStringTypes.Readable));
            }
        }

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("name", this.RuntimeMethod.GetReadableName()) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                this.RuntimeMethod.GetReadableName());

        [DebuggerStepThrough]
        public static MethodTerm From(MethodBase method) =>
            new MethodTerm(method);
    }

    public static class MethodTermExtension
    {
        [DebuggerStepThrough]
        public static void Deconstruct(
            this IMethodTerm method,
            out MethodBase runtimeMethod) =>
            runtimeMethod = method.RuntimeMethod;
    }
}
