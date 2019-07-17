using System;

namespace Favalet
{
    public sealed class TextRange : IEquatable<TextRange>
    {
        private static readonly Uri unknown = new Uri("unknown.fav", UriKind.RelativeOrAbsolute);

        public static readonly TextRange Unknown = new TextRange(unknown, Range.Empty);

        public readonly Uri Target;
        public readonly Range Range;

        private TextRange(string target, Range range)
        {
            this.Target = new Uri(target, UriKind.RelativeOrAbsolute);
            this.Range = range;
        }

        private TextRange(Uri target, Range range)
        {
            this.Target = target;
            this.Range = range;
        }

        public bool Contains(TextRange inside) =>
            this.Target.Equals(inside.Target) && this.Range.Contains(inside.Range);

        public override string ToString() =>
            $"{(this.Target.IsFile ? this.Target.LocalPath : this.Target.ToString())}({this.Range})";

        public override int GetHashCode() =>
            this.Target.GetHashCode() ^ this.Range.GetHashCode();

        public bool Equals(TextRange other) =>
            this.Target.Equals(other.Target) && this.Range.Equals(other.Range);

        bool IEquatable<TextRange>.Equals(TextRange other) =>
            this.Target.Equals(other.Target) && this.Range.Equals(other.Range);

        public override bool Equals(object obj) =>
            obj is TextRange textRange ? this.Equals(textRange) : false;

        public static TextRange Create(string target, Range range) =>
            new TextRange(target, range);
        public static TextRange Create(Uri target, Range range) =>
            new TextRange(target, range);
        public static TextRange Create(Range range) =>
            new TextRange(unknown, range);

        public static implicit operator TextRange(System.ValueTuple<string, int, int> textRange) =>
            new TextRange(textRange.Item1, Range.Create(Position.Create(textRange.Item2, textRange.Item3)));
        public static implicit operator TextRange(System.ValueTuple<Uri, int, int> textRange) =>
           new TextRange(textRange.Item1, Range.Create(Position.Create(textRange.Item2, textRange.Item3)));
        public static implicit operator TextRange(System.ValueTuple<string, int, int, int, int> textRange) =>
            new TextRange(textRange.Item1, Range.Create(Position.Create(textRange.Item2, textRange.Item3), Position.Create(textRange.Item4, textRange.Item5)));
        public static implicit operator TextRange(System.ValueTuple<Uri, int, int, int, int> textRange) =>
            new TextRange(textRange.Item1, Range.Create(Position.Create(textRange.Item2, textRange.Item3), Position.Create(textRange.Item4, textRange.Item5)));
    }
}
