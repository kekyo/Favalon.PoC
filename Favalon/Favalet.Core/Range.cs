// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Favalet
{
    public struct Range
    {
        public static readonly Range Empty = Create(Position.Empty, Position.Empty);
        public static readonly Range MaxValue = Create(Position.Empty, Position.MaxValue);

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

        public bool Overlaps(Range rhs) =>
            (this.First <= rhs.Last) && (this.Last >= rhs.First);

        public Range Combine(Range range) =>
            this.Combine(range.First, range.Last);
        public Range Combine(Position first, Position last) =>
            new Range((this.First < first) ? this.First : first, (this.Last < last) ? this.Last : last);

        public Range Subtract(Range range) =>
            this.Subtract(range.First, range.Last);
        public Range Subtract(Position first, Position last) =>
            new Range(this.Contains(Create(first)) ? first : this.First, this.Contains(Create(last)) ? last : this.Last);

        public override string ToString() =>
            (this.First.Equals(this.Last)) ?
                this.First.ToString() :
                $"{this.First},{this.Last}";

        public static Range Create(Position position) =>
            new Range(position, position);
        public static Range Create(Position first, Position last) =>
            new Range(first, last);

        public static implicit operator Range((int line, int column) position) =>
            Create(Position.Create(position.line, position.column));
        public static implicit operator Range((int lineFirst, int columnFirst, int lineLast, int columnLast) range) =>
            Create(Position.Create(range.lineFirst, range.columnFirst), Position.Create(range.lineLast, range.columnLast));
    }
}
