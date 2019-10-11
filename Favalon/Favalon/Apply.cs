using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Apply : Term, IEquatable<Apply>
    {
        public readonly Term Function;
        public readonly Term Argument;

        public Apply(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term HigherOrder =>
            this.Function.HigherOrder is Function fho ?
                fho.Body :
                Unspecified.Instance;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();

        public bool Equals(Apply? other) =>
            (other?.Function.Equals(this.Function) ?? false) &&
            (other?.Argument.Equals(this.Argument) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Term? other) =>
            this.Equals(other as Apply);

        public override string ToString() =>
            $"{this.Function} {this.Argument}";

        public override Term VisitInfer(Environment environment)
        {
            var function = this.Function.VisitInfer(environment);
            var argument = this.Argument.VisitInfer(environment);

            return
                (object.ReferenceEquals(function, this.Function) &&
                 object.ReferenceEquals(argument, this.Argument)) ?
                    this :
                    new Apply(function, argument);
        }
    }
}
