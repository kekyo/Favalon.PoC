using System;
using System.Collections.Generic;

namespace LambdaCalculus
{
    public sealed class LambdaOperatorTerm : ApplicableTerm
    {
        private LambdaOperatorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new LambdaArrowParameterTerm(((IdentityTerm)rhs.Reduce(context)).Identity);

        public override Term Infer(Context context) =>
            this;

        public override bool Equals(Term? other) =>
            other is LambdaOperatorTerm;

        public static LambdaOperatorTerm Instance =
            new LambdaOperatorTerm();

        private sealed class LambdaArrowParameterTerm : ApplicableTerm
        {
            public readonly string Parameter;

            public LambdaArrowParameterTerm(string parameter) =>
                this.Parameter = parameter;

            public override Term HigherOrder =>
                Lambda("?", UnspecifiedTerm.Instance);

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                new LambdaTerm(this.Parameter, rhs);

            public override Term Infer(Context context) =>
                this;

            public override bool Equals(Term? other) =>
                other is LambdaArrowParameterTerm rhs ? this.Parameter.Equals(rhs.Parameter) : false;
        }
    }

    public sealed class LambdaTerm : ApplicableTerm
    {
        public readonly string Parameter;
        public readonly Term Body;

        internal LambdaTerm(string parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term HigherOrder =>
            this.Body.HigherOrder;

        public override Term Reduce(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Reduce(context));

        protected internal override Term? Apply(Context context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.AddBoundTerm(this.Parameter, rhs);

            return this.Body.Reduce(newScope);
        }

        public override Term Infer(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Infer(context));

        public override bool Equals(Term? other) =>
            other is LambdaTerm rhs ?
                (this.Parameter.Equals(rhs.Parameter) && this.Body.Equals(rhs.Body)) :
                false;
    }
}
