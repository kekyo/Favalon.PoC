using System;

namespace Favalon.Expressions
{
    public abstract class Expression : IEquatable<Expression?>
    {
        private protected Expression()
        { }

        public abstract Expression HigherOrder { get; }

        public abstract bool Equals(Expression? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as Expression);

        public abstract Expression Run();
    }
}
