﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions
{
    public interface IIdentityTerm : ITerm
    {
        string Symbol { get; }
    }

    public sealed class IdentityTerm :
        Expression, IIdentityTerm
    {
        public readonly string Symbol;

        private IdentityTerm(string symbol) =>
            this.Symbol = symbol;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Symbol =>
            this.Symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IIdentityTerm rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context) =>
            context.LookupVariable(this.Symbol) ?? this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => this.Symbol,
                _ => $"(Identity {this.Symbol})"
            };

        public static IdentityTerm Create(string symbol) =>
            new IdentityTerm(symbol);
    }
}
