namespace Favalet
{
    public struct Position
    {
        public static readonly Position Empty = new Position(0, 0);

        public readonly int Line;
        public readonly int Column;

        private Position(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public override string ToString() =>
            $"{this.Line},{this.Column}";

        public static Position Create(int line, int column) =>
            new Position(line, column);

        public static implicit operator Position(System.ValueTuple<int, int> position) =>
            new Position(position.Item1, position.Item2);
    }
}
