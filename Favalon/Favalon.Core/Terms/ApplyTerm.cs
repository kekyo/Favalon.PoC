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

        private struct TransposeResult0
        {
            public readonly Term Result;
            public readonly bool WillTranspose;

            public TransposeResult0(Term result, bool willTranspose)
            {
                this.Result = result;
                this.WillTranspose = willTranspose;
            }

            public void Deconstruct(out Term result, out bool willTranspose)
            {
                result = this.Result;
                willTranspose = this.WillTranspose;
            }
        }

        private struct TransposeResult1
        {
            public readonly Term Result;
            public readonly Term? Right;
            public readonly bool WillTranspose;

            public TransposeResult1(Term result, Term? right, bool willTranspose)
            {
                this.Result = result;
                this.Right = right;
                this.WillTranspose = willTranspose;
            }

            public void Deconstruct(out Term result, out Term? right, out bool willTranspose)
            {
                result = this.Result;
                right = this.Right;
                willTranspose = this.WillTranspose;
            }
        }

        private static TransposeResult1 InternalTranspose1(Context context, Term term)
        {
            if (term is ApplyTerm(Term function, Term argument))
            {
                var leftResult = InternalTranspose1(context, function);
                var left = leftResult.Result;
                var willTranspose = leftResult.WillTranspose;

                var right = context.Transpose(argument);

                var rightToLeft = false;

                if (leftResult.Right is Term rightTerm)
                {
                    var rightResult = InternalTranspose0(context, new ApplyTerm(rightTerm, right));

                    right = rightResult.Result;

                    // TODO: validate this expression
                    willTranspose = willTranspose || rightResult.WillTranspose;
                }

                // Swap by infix variables
                if (right is VariableTerm rightVariable &&
                    context.LookupBoundTerms(rightVariable) is BoundTermInformation[] rightTerms)
                {
                    if (rightTerms[0].Infix)
                    {
                        // Rule 2: abc def + ==> abc + def
                        if (left is ApplyTerm(Term applyLeft1, Term applyRight1))
                        {
                            right = applyRight1;
                            left = new ApplyTerm(applyLeft1, rightVariable);
                        }
                        // Rule 1: abc + ==> + abc
                        else
                        {
                            right = left; // swap
                            left = rightVariable;
                        }
                    }

                    if (rightTerms[0].RightToLeft)
                    {
                        rightToLeft = true;
                    }
                }
                else if (left is VariableTerm leftVariable &&
                    context.LookupBoundTerms(leftVariable) is BoundTermInformation[] leftTerms)
                {
                    if (leftTerms[0].RightToLeft)
                    {
                        rightToLeft = true;
                    }
                }

                if (willTranspose)
                {
                    return new TransposeResult1(left, right, rightToLeft || willTranspose);
                }

                // If changed left and/or right terms
                if (!object.ReferenceEquals(left, function) ||
                    !object.ReferenceEquals(right, argument))
                {
                    return new TransposeResult1(new ApplyTerm(left, right), null, rightToLeft);
                }
                else
                {
                    return new TransposeResult1(term, null, rightToLeft);
                }
            }
            else
            {
                return new TransposeResult1(context.Transpose(term), null, false);
            }
        }

        private static TransposeResult0 InternalTranspose0(Context context, Term term)
        {
            var result = InternalTranspose1(context, term);

            if (result is TransposeResult1(Term left, Term right, bool willTranspose))
            {
                return new TransposeResult0(new ApplyTerm(left, right), willTranspose);
            }
            else
            {
                return new TransposeResult0(result.Result, result.WillTranspose);
            }
        }

        protected internal override Term VisitTranspose(Context context) =>
            InternalTranspose0(context, this).Result;

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
