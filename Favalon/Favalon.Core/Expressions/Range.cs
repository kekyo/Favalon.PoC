using System;

namespace Favalon.Expressions
{
    public struct Range
    {
        private static readonly Uri unknown = new Uri("unknown", UriKind.RelativeOrAbsolute);

        public static readonly Range Unknown = Create(Position.Empty, Position.Empty);

        public readonly Uri Target;
        public readonly Position First;
        public readonly Position Last;

        private Range(Uri target, Position first, Position last)
        {
            this.Target = target;
            this.First = first;
            this.Last = last;
        }

        public override string ToString() =>
            (this.First.Equals(this.Last)) ?
            $"{this.Target.LocalPath}({this.First})" :
            $"{this.Target.LocalPath}({this.First},{this.Last})";

        public static Range Create(Uri target, Position first, Position last) =>
            new Range(target, first, last);
        public static Range Create(Position first, Position last) =>
            new Range(unknown, first, last);
    }
}
