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

        public TextRange Combine(Range rhs) =>
            new TextRange(this.Target, this.Range.Combine(rhs));
        public TextRange Combine(Position first, Position last) =>
            new TextRange(this.Target, this.Range.Combine(first, last));
        public TextRange Combine(TextRange rhs) =>
            this.Target.Equals(rhs.Target) ?
                new TextRange(this.Target, this.Range.Combine(rhs.Range)) :
                throw new InvalidOperationException();

        public TextRange Subtract(Range range) =>
            new TextRange(this.Target, this.Range.Subtract(range));
        public TextRange Subtract(Position first, Position last) =>
            new TextRange(this.Target, this.Range.Subtract(first, last));
        public TextRange Subtract(TextRange rhs) =>
            this.Target.Equals(rhs.Target) ?
                new TextRange(this.Target, this.Range.Subtract(rhs.Range)) :
                throw new InvalidOperationException();

        public override string ToString() =>
            $"{((this.Target.IsAbsoluteUri && this.Target.IsFile) ? this.Target.LocalPath : this.Target.ToString())}({this.Range})";

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

        public static implicit operator TextRange((string target, int line, int column) textRange) =>
            new TextRange(textRange.target, Range.Create(Position.Create(textRange.line, textRange.column)));
        public static implicit operator TextRange((Uri target, int line, int column) textRange) =>
           new TextRange(textRange.target, Range.Create(Position.Create(textRange.line, textRange.column)));
        public static implicit operator TextRange((string target, int lineFirst, int columnFirst, int lineLast, int columnLast) textRange) =>
            new TextRange(textRange.target, Range.Create(Position.Create(textRange.lineFirst, textRange.columnFirst), Position.Create(textRange.lineLast, textRange.columnLast)));
        public static implicit operator TextRange((Uri target, int lineFirst, int columnFirst, int lineLast, int columnLast) textRange) =>
            new TextRange(textRange.target, Range.Create(Position.Create(textRange.lineFirst, textRange.columnFirst), Position.Create(textRange.lineLast, textRange.columnLast)));
    }
}
