﻿using Favalet.Contexts;
using Favalet.Expressions;
using System;
using System.Threading;

namespace Favalet
{
    // For testing purpose.
    internal sealed class PseudoPlaceholderProvider
    {
        private volatile int pseudoIndex = -1;

        private PseudoPlaceholderProvider()
        { }

        public ITerm CreatePlaceholder() =>
            new PseudoPlaceholderTerm(Interlocked.Increment(ref this.pseudoIndex));

        public static PseudoPlaceholderProvider Create() =>
            new PseudoPlaceholderProvider();

        internal sealed class PseudoPlaceholderTerm :
            Expression, ITerm
        {
            public readonly int PseudoIndex;

            public PseudoPlaceholderTerm(int pseudoIndex) =>
                this.PseudoIndex = pseudoIndex;

            public override IExpression HigherOrder =>
                throw new NotImplementedException();

            public override int GetHashCode() =>
                this.PseudoIndex.GetHashCode();

            public bool Equals(PseudoPlaceholderTerm rhs) =>
                throw new NotImplementedException();

            public override bool Equals(IExpression? other) =>
                throw new NotImplementedException();

            public override string GetPrettyString(PrettyStringContext context) =>
                $"'{this.PseudoIndex}";

            protected override IExpression Fixup(IReduceContext context) =>
                throw new NotImplementedException();

            protected override IExpression Infer(IReduceContext context) =>
                throw new NotImplementedException();

            protected override IExpression Reduce(IReduceContext context) =>
                throw new NotImplementedException();
        }
    }
}
