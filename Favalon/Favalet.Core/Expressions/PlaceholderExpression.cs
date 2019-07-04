﻿using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class PlaceholderExpression : Expression, IEquatable<PlaceholderExpression?>, IComparable<PlaceholderExpression>
    {
        public PlaceholderExpression(int index, Expression higherOrder) :
            base(higherOrder) =>
        this.Index = index;

        public readonly int Index;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            switch (context.FormatNaming)
            {
                case FormatNamings.Strict:
                    return $"'{this.Index}";
                default:
                    var index = context.GetAdjustedIndex(this);
                    var ch = (char)('a' + (index % ('z' - 'a' + 1)));
                    var suffixIndex = index / ('z' - 'a' + 1);
                    var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                    return $"'{ch}{suffix}";
            }
        }

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(Environment environment)
        {
            return this;
        }

        public override int GetHashCode() =>
            this.Index;

        public bool Equals(PlaceholderExpression? other) =>
            other?.Index.Equals(this.Index) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as PlaceholderExpression);

        public int CompareTo(PlaceholderExpression other) =>
            this.Index.CompareTo(other.Index);
    }
}
