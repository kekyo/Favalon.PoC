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

        protected internal override Term VisitTranspose(Context context)
        {
            var left = context.Transpose(this.Function);
            var right = context.Transpose(this.Argument);

            switch (left)
            {
                case ApplyTerm(Term applyLeft, Term applyRight):
                    left = applyLeft;
                    right = new ApplyTerm(applyRight, right);
                    break;
            }

            switch (right)
            {
                case VariableTerm variable when
                    (context.LookupBoundTerms(variable) is BoundTerm[] terms && terms[0].Associative == BoundAssociatives.Right):
                    return new ApplyTerm(variable, left);  // tramsposed between left and right.
            }

            if (!object.ReferenceEquals(left, this.Function) ||
                !object.ReferenceEquals(right, this.Argument))
            {
                return new ApplyTerm(left, right);
            }
            else
            {
                return this;
            }
        }

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
