using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Terms
{
    public sealed class ExecutableSymbol : Symbol
    {
        public readonly string Path;

        internal ExecutableSymbol(string path) :
            base(Unspecified.Instance) => // TODO: Kind
            this.Path = path;

        public override string PrintableName =>
            System.IO.Path.GetFileNameWithoutExtension(this.Path);

        public override int GetHashCode() =>
            this.Path.GetHashCode();

        public bool Equals(ExecutableSymbol? other) =>
            other?.Path.Equals(this.Path) ?? false;

        public override bool Equals(Symbol? other) =>
            this.Equals(other as ExecutableSymbol);

        public override Expression VisitInfer(Environment environment) =>
            throw new InvalidOperationException();
    }
}
