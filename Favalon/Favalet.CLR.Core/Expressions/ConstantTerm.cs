﻿using Favalet.Contexts;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Favalet.Expressions
{
    public interface IConstantTerm : ITerm
    {
        object Value { get; }
    }

    public sealed class ConstantTerm :
        Expression, IConstantTerm
    {
        public readonly object Value;

        private ConstantTerm(object value) =>
            this.Value = value;

        public IExpression HigherOrder =>
            TypeTerm.From(this.Value.GetType());

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IConstantTerm.Value =>
            this.Value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(IConstantTerm rhs) =>
            this.Value.Equals(rhs.Value);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IConstantTerm rhs && this.Equals(rhs);

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            this.Value switch
            {
                string value => $"\"{value}\"",
                _ => this.Value.ToString()
            };

        public static ConstantTerm From(object value) =>
            new ConstantTerm(value);
    }
}
