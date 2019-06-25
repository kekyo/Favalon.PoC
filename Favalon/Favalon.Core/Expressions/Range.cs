namespace Favalon.Expressions
{
    public struct Range
    {
        public static readonly Range Empty = Create(Position.Empty, Position.Empty);

        public readonly Position First;
        public readonly Position Last;

        private Range(Position first, Position last)
        {
            this.First = first;
            this.Last = last;
        }

        public bool Contains(Range inside) =>
            ((this.First.Line < inside.First.Line) || ((this.First.Line == inside.First.Line) && (this.First.Column <= inside.First.Column))) &&
            ((inside.Last.Line < this.Last.Line) || ((inside.Last.Line == this.Last.Line) && (inside.Last.Column <= this.Last.Column)));

        public override string ToString() =>
            (this.First.Equals(this.Last)) ?
                this.First.ToString() :
                $"{this.First},{this.Last}";

        public static Range Create(Position position) =>
            new Range(position, position);
        public static Range Create(Position first, Position last) =>
            new Range(first, last);
    }
}
