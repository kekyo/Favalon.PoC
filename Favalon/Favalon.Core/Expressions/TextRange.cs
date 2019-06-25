using System;

namespace Favalon.Expressions
{
    public sealed class TextRange
    {
        private static readonly Uri unknown = new Uri("unknown", UriKind.RelativeOrAbsolute);

        public static readonly TextRange Unknown = new TextRange(unknown, Range.Empty);

        public readonly Uri Target;
        public readonly Range Range;

        private TextRange(Uri target, Range range)
        {
            this.Target = target;
            this.Range = range;
        }

        public bool Contains(TextRange inside) =>
            this.Target.Equals(inside.Target) && this.Range.Contains(inside.Range);

        public override string ToString() =>
            $"{(this.Target.IsFile ? this.Target.LocalPath : this.Target.ToString())}({this.Range})";

        public static TextRange Create(Uri target, Range range) =>
            new TextRange(target, range);
        public static TextRange Create(Range range) =>
            new TextRange(unknown, range);
    }
}
