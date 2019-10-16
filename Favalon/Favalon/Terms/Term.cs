﻿using Favalon.Expressions;
using System;

namespace Favalon.Terms
{
    public abstract class Term : IEquatable<Term?>
    {
        private protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract bool Equals(Term? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as Term);

        public abstract Expression VisitInfer(Environment environment);
    }
}
