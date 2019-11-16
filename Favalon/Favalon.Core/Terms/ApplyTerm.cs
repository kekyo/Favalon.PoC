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

        private static Term InternalTranspose(Context context, Term leftTerm)
        {
            if (leftTerm is ApplyTerm(Term function, Term argument))
            {
                var left = InternalTranspose(context, function);
                var right = context.Transpose(argument);

                switch (right)
                {
                    // Swap by infix variables
                    case VariableTerm variable when
                        context.LookupBoundTerms(variable) is BoundTermInformation[] terms && terms[0].Infix:
                        // abc def + ==> abc + def
                        if (left is ApplyTerm(Term applyLeft, Term applyRight))
                        {
                            right = applyRight; // swap
                            left = new ApplyTerm(applyLeft, variable);
                        }
                        // abc + ==> + abc
                        else
                        {
                            right = left; // swap
                            left = variable;
                        }
                        break;
                }

                switch (left)
                {
                    // Transpose by right associative variables
                    // abc -> def ghi ==> -> abc (def ghi)
                    case ApplyTerm(ApplyTerm(VariableTerm applyLeft, Term _) apply, Term applyRight) when
                        context.LookupBoundTerms(applyLeft) is BoundTermInformation[] terms && terms[0].RightToLeft:
                        right = new ApplyTerm(applyRight, right);
                        left = apply;
                        break;
                }

                if (!object.ReferenceEquals(left, function) ||
                    !object.ReferenceEquals(right, argument))
                {
                    return new ApplyTerm(left, right);
                }
                else
                {
                    return leftTerm;
                }
            }
            else
            {
                return context.Transpose(leftTerm);
            }
        }

        protected internal override Term VisitTranspose(Context context) =>
            InternalTranspose(context, this);

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
            return this.Argument is VariableTerm ?
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
