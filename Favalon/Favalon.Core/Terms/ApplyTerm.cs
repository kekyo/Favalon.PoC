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

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            new ApplyTerm(
                this.Function.VisitReplace(identity, replacement),
                this.Argument.VisitReplace(identity, replacement));

        private struct TranslateFunctionCandidates
        {
            public readonly Term Parameter;
            public readonly Term Argument;
            public readonly Term? ApplyingFunction;

            public TranslateFunctionCandidates(Term parameter, Term argument, Term? applyingFunction = default)
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

        private static readonly IdentityTerm functionIdentity = new IdentityTerm("->");

        private static TranslateFunctionCandidates? TranslateFunction(
            Context context, Term term)
        {
            switch (term)
            {
                case ApplyTerm(ApplyTerm(ApplyTerm(Term f, IdentityTerm identity), Term p), Term a0)
                    when context.LookupIdentity(identity) is Term[] terms:
                    return new TranslateFunctionCandidates(p, a0, f);
                case ApplyTerm(ApplyTerm(IdentityTerm identity, Term p), Term a0)
                    when context.LookupIdentity(identity) is Term[] terms:
                    return new TranslateFunctionCandidates(p, a0);
                case ApplyTerm(Term child, Term a1):
                    if (TranslateFunction(context, child) is TranslateFunctionCandidates candidates)
                    {
                        return new TranslateFunctionCandidates(
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

        internal Term VisitReduce0(Context context)
        {
            Term function;
            Term argument;

            switch (TranslateFunction(context, this))
            {
                case TranslateFunctionCandidates(Term p, Term a, Term af):
                    function = af.VisitReduce(context);
                    argument = new FunctionTerm(p.VisitReduce(context), a.VisitReduce(context));
                    break;
                case TranslateFunctionCandidates(Term p, Term a, null):
                    return new FunctionTerm(p.VisitReduce(context), a.VisitReduce(context));
                default:
                    function = this.Function.VisitReduce(context);
                    argument = this.Argument.VisitReduce(context);
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

        public override string ToString()
        {
            var function = this.Function is FunctionTerm ?
                $"({this.Function})" :
                this.Function.ToString();
            return this.Argument is IdentityTerm ?
                $"{function} {this.Argument}" :
                $"{function} ({this.Argument})";
        }

        public void Deconstruct(out Term function, out Term argument)
        {
            function = this.Function;
            argument = this.Argument;
        }
    }
}
