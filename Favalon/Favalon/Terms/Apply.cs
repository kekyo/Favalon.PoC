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

        public Apply(Term function, Term argument)
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

        public override Term VisitInfer(Environment environment)
        {
            var function = Function.VisitInfer(environment);
            var argument = Argument.VisitInfer(environment);

            return
                ReferenceEquals(function, Function) &&
                 ReferenceEquals(argument, Argument) ?
                    this :
                    new Apply(function, argument);
        }

        public override object Reduce() =>
            ((Func<object, object>)Function.Reduce())(Argument.Reduce());
    }
}
