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

        private struct TransposeCandidates
        {
            public readonly Term Parameter;
            public readonly Term Argument;
            public readonly Term? ApplyingFunction;

            public TransposeCandidates(Term parameter, Term argument, Term? applyingFunction = default)
            {
                this.Parameter = parameter;
                this.Argument = argument;
                this.ApplyingFunction = applyingFunction;
            }

            public void Deconstruct(out Term parameter, out Term argument, out Term? applyingFunction)
            {
                parameter = this.Parameter;
                argument = this.Argument;
                applyingFunction = this.ApplyingFunction;
            }
        }

        private static TransposeCandidates? TransposeOperator(
            Context context, Term term)
        {
            switch (term)
            {
                case ApplyTerm(ApplyTerm(Term _, Term p), Term a0):
                    return new TransposeCandidates(p, a0);
                case ApplyTerm(Term child, Term a1):
                    if (TransposeOperator(context, child) is TransposeCandidates candidates)
                    {
                        return new TransposeCandidates(
                            candidates.Parameter,
                            new ApplyTerm(candidates.Argument, a1),   // a0, a1
                            candidates.ApplyingFunction);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        protected internal override Term VisitTranspose(Context context)
        {
            Term function;
            Term argument;

            switch (TransposeOperator(context, this))
            {
                case TransposeCandidates(Term p, Term a, Term af):
                    function = context.Transpose(af);
                    argument = new FunctionTerm(context.Transpose(p), context.Transpose(a));
                    break;
                case TransposeCandidates(Term p, Term a, null):
                    return new FunctionTerm(context.Transpose(p), context.Transpose(a));
                default:
                    function = context.Transpose(this.Function);
                    argument = context.Transpose(this.Argument);
                    break;
            }

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
