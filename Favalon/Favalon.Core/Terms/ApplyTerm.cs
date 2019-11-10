using System;

namespace Favalon.Terms
{
    public sealed class ApplyTerm :
        Term, IEquatable<ApplyTerm?>
    {
        public new readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        private struct TransposeResult
        {
            public readonly Term Term;
            public readonly bool Swapped;
            public readonly bool WillTranspose;

            public TransposeResult(Term term, bool swapped, bool willTranspose)
            {
                this.Term = term;
                this.Swapped = swapped;
                this.WillTranspose = willTranspose;
            }

            public void Deconstruct(out Term term, out bool swapped, out bool willTranspose)
            {
                term = this.Term;
                swapped = this.Swapped;
                willTranspose = this.WillTranspose;
            }
        }

        private static TransposeResult Transpose(Context context, Term term) =>
            term is ApplyTerm apply ?
                Transpose(context, apply) :
                new TransposeResult(context.Transpose(term), false, false);

        private static TransposeResult Transpose(Context context, ApplyTerm term)
        {
            var leftTransposed = Transpose(context, term.Function);
            var rightTransposed = Transpose(context, term.Argument);

            var left = leftTransposed.Term;
            var right = rightTransposed.Term;

            switch (leftTransposed)
            {
                case TransposeResult(ApplyTerm(Term applyLeft, Term applyRight), _, true):
                    left = applyLeft;   // transposed
                    right = new ApplyTerm(applyRight, right);
                    break;
            }

            switch (right)
            {
                case VariableTerm variable when
                    (context.LookupBoundTerms(variable) is BoundTerm[] terms && terms[0].Associative == BoundAssociatives.Right):
                    return new TransposeResult(new ApplyTerm(variable, left), true, leftTransposed.Swapped);  // swapped left and right, will transpose if child is swapped
            }

            if (!object.ReferenceEquals(left, term.Function) ||
                !object.ReferenceEquals(right, term.Argument))
            {
                return new TransposeResult(new ApplyTerm(left, right), false, leftTransposed.Swapped);  // will transpose if child is swapped
            }
            else
            {
                return new TransposeResult(term, false, false);
            }
        }

        protected internal override Term VisitTranspose(Context context) =>
            Transpose(context, this).Term;

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            new ApplyTerm(
                this.Function.VisitReplace(identity, replacement),
                this.Argument.VisitReplace(identity, replacement));

        protected internal override Term VisitReduce(Context context)
        {
            var function = this.Function.VisitReduce(context);
            var argument = this.Argument.VisitReduce(context);

            if (function is CallableTerm callable)
            {
                return callable.VisitCall(context, argument);
            }
            else if (
                !object.ReferenceEquals(function, this.Function) ||
                !object.ReferenceEquals(argument, this.Argument))
            {
                return new ApplyTerm(function, argument);
            }
            else
            {
                return this;
            }
        }

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^
            this.Argument.GetHashCode();

        public bool Equals(ApplyTerm? other) =>
            (other?.Function.Equals(this.Function) ?? false) &&
            (other?.Argument.Equals(this.Argument) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as ApplyTerm);

        protected override string VisitTermString(bool includeTermName)
        {
            var function = this.Function is FunctionTerm ?
                $"({this.Function.ToString(includeTermName)})" :
                this.Function.ToString(includeTermName);
            return this.Argument is IdentityTerm ?
                $"{function} {this.Argument.ToString(includeTermName)}" :
                $"{function} ({this.Argument.ToString(includeTermName)})";
        }

        public void Deconstruct(out Term function, out Term argument)
        {
            function = this.Function;
            argument = this.Argument;
        }
    }
}
