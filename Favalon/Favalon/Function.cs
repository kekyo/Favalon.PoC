using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Function : Term, IEquatable<Function>
    {
        public readonly Term Parameter;
        public readonly Term Body;

        public Function(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
            this.HigherOrder = Unspecified.Instance; //new Function(parameter.HigherOrder, body.HigherOrder);
        }

        public override Term HigherOrder { get; }

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public bool Equals(Function? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Body.Equals(this.Body) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Term? other) =>
            this.Equals(other as Function);

        public override string ToString() =>
            $"{this.Parameter} -> {this.Body}";
    }
}
