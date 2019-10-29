using System;

namespace Favalon.Terms
{
    public sealed class ConstantTerm :
        Term, IEquatable<ConstantTerm?>
    {
        public new readonly object Constant;

        internal ConstantTerm(object constant) =>
            this.Constant = constant;

        public override Term VisitReplace(string identity, Term replacement) =>
            this;

        public override Term VisitReduce() =>
            this;

        public override int GetHashCode() =>
            this.Constant?.GetHashCode() ?? 0;

        public bool Equals(ConstantTerm? other) =>
            other?.Constant.Equals(this.Constant) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as ConstantTerm);

        public override string ToString() =>
            this.Constant is string stringValue ?
                "\"" + this.Constant + "\"" :
            this.Constant?.ToString() ?? "null";

        public void Deconstruct(out object constant) =>
            constant = this.Constant;
    }
}
