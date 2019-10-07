using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Variable : Term, IEquatable<Variable?>
    {
        public readonly string Name;

        internal Variable(string name) =>
            this.Name = name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(Variable? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as Variable);
    }
}
