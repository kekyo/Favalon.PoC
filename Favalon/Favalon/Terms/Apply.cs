using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public sealed class Apply : Term, IEquatable<Apply>
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal Apply(Term function, Term argument)
        {
            Function = function;
            Argument = argument;
            HigherOrder = Factories.Function(
                Function.HigherOrder, Argument.HigherOrder);
        }

        public override Term HigherOrder { get; }

        public override int GetHashCode() =>
            Function.GetHashCode() ^ Argument.GetHashCode();

        public bool Equals(Apply? other) =>
            (other?.Function.Equals(Function) ?? false) &&
            (other?.Argument.Equals(Argument) ?? false) &&
            (other?.HigherOrder.Equals(HigherOrder) ?? false);

        public override bool Equals(Term? other) =>
            Equals(other as Apply);

        public override string ToString() =>
            Argument is Apply apply ?
                $"{Function} ({apply})" :
                $"{Function} {Argument}";

        public override Expression VisitInfer(Environment environment)
        {
            var argumentExpression = this.Argument.VisitInfer(environment);

            var function = this.Function switch
            {
                Variable variable when environment.Lookup(variable.Name) is Term term => term,
                Term term => term
            };

            return function switch
            {
                MethodSymbol methodSymbol => new CallMethod(methodSymbol.Method, argumentExpression),
                ExecutableSymbol executableSymbol => new RunExecutable(executableSymbol.Path, argumentExpression),
                _ => throw new ArgumentException()
            };
        }
    }
}
