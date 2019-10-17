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
            Expression visit(Term function) =>
                function switch
                {
                    MethodSymbol methodSymbol =>
                        new CallMethod(methodSymbol.Method, this.Argument.VisitInfer(environment)),
                    ExecutableSymbol executableSymbol =>
                        new RunExecutable(executableSymbol.Path, this.Argument.VisitInfer(environment)),
                    TypeSymbol typeSymbol =>
                        new CallMethod(typeSymbol.Type.GetConstructor(
                            new[] { ((TypeSymbol)this.Argument.HigherOrder).Type.AsType() }),
                            this.Argument.VisitInfer(environment)),
                    _ => throw new ArgumentException()
                };

            var function = this.Function;
            switch (function)
            {
                case Variable variable:
                    var terms = environment.Lookup(variable.Name);
                    return visit(terms[0]);  // TODO: choice overloads.
                default:
                    return visit(function);
            }
        }
    }
}
