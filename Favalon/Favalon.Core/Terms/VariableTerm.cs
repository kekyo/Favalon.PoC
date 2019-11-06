using System;

namespace Favalon.Terms
{
    public abstract class VariableTerm :
        Term, IEquatable<VariableTerm?>
    {
        public readonly string Name;

        protected VariableTerm(string name) =>
            this.Name = name;

        protected internal override sealed Term VisitReduce(Context context) =>
            context.LookupBoundTerms(this) is Term[] terms ?
                terms[0] :
                this;

        public override sealed int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(VariableTerm? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override sealed bool Equals(object obj) =>
            this.Equals(obj as VariableTerm);

        protected override sealed string VisitTermString(bool includeTermName) =>
            this.Name;

        public void Deconstruct(out string name) =>
            name = this.Name;
    }
}
