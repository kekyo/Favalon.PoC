using System;

namespace BasicSyntaxTree
{
    public struct TextRegion
    {
        public readonly Uri Target;
        public readonly int BeginRow;
        public readonly int BeginColumn;
        public readonly int EndRow;
        public readonly int EndColumn;

        private TextRegion(
            Uri target, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            this.Target = target;
            this.BeginRow = beginRow;
            this.BeginColumn = beginColumn;
            this.EndRow = endRow;
            this.EndColumn = endColumn;
        }

        public override string ToString() =>
            $"{this.Target.PathAndQuery}({this.BeginRow},{this.BeginColumn})";

        public static readonly TextRegion Unknown =
            new TextRegion(new Uri("unknown.fav", UriKind.RelativeOrAbsolute), 0, 0, 1, 1);

        public static TextRegion Create(
            Uri target, int beginRow, int beginColumn, int endRow, int endColumn) =>
            new TextRegion(target, beginRow, beginColumn, endRow, endColumn);
    }
}
