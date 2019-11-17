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
            public readonly Term Result;
            public readonly Term? TransposeTarget;

            public TransposeResult(Term result, Term? transposeTarget)
            {
                this.Result = result;
                this.TransposeTarget = transposeTarget;
            }
        }

        private static Term InternalTranspose(Context context, Term term) =>
            InternalTranspose_(context, term).Result;

        private static TransposeResult InternalTranspose_(Context context, Term term)
        {
            if (term is ApplyTerm(Term function, Term argument))
            {
                var leftResult = InternalTranspose_(context, function);
                var left = leftResult.Result;
                Term? transposeTarget = null;

                //var left = InternalTranspose_(context, function).Result;

                var right = context.Transpose(argument);

                // Swap by infix variables
                switch (right)
                {
                    case VariableTerm variable:
                        if (context.LookupBoundTerms(variable) is BoundTermInformation[] terms1)
                        {
                            if (terms1[0].Infix)
                            {
                                // abc def + ==> abc + def
                                if (left is ApplyTerm(Term applyLeft1, Term applyRight1))
                                {
                                    right = applyRight1; // swap
                                    left = new ApplyTerm(applyLeft1, variable);
                                }
                                // abc + ==> + abc
                                else
                                {
                                    right = left; // swap
                                    left = variable;
                                }
                            }
                        }
                        break;
                }

                switch (left)
                {
                    case ApplyTerm(VariableTerm applyLeft2, Term _):
                        if (context.LookupBoundTerms(applyLeft2) is BoundTermInformation[] terms2)
                        {
                            if (terms2[0].RightToLeft)
                            {
                                transposeTarget = right;
                            }
                        }
                        break;
                    case ApplyTerm(Term applyLeft, Term applyRight):
                        // Transpose by right associative variables
                        // abc -> def ghi ==> -> abc (def ghi)
                        if (leftResult.TransposeTarget is Term lastTransposeTarget)
                        {
                            right = new ApplyTerm(lastTransposeTarget, applyRight);
                            left = applyLeft;
                            transposeTarget = right;
                        }
                        break;
                }

                // Transpose by right associative variables
                // abc -> def ghi ==> -> abc (def ghi)
                //if (left is ApplyTerm(ApplyTerm(VariableTerm applyLeft2, Term _) apply, Term applyRight2))
                //{
                //    if (context.LookupBoundTerms(applyLeft2) is BoundTermInformation[] terms && terms[0].RightToLeft)
                //    {
                //        right = new ApplyTerm(applyRight2, right);
                //        left = apply;
                //    }
                //}

                // If changed left and/or right terms
                if (!object.ReferenceEquals(left, function) ||
                    !object.ReferenceEquals(right, argument))
                {
                    return new TransposeResult(new ApplyTerm(left, right), transposeTarget);
                }
                else
                {
                    return new TransposeResult(term, transposeTarget);
                }
            }
            else
            {
                return new TransposeResult(context.Transpose(term), null);
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
