using System;

namespace Favalon.Expressions
{
#pragma warning disable CS0659
    public abstract class Expression : IEquatable<Expression?>
#pragma warning restore CS0659
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
