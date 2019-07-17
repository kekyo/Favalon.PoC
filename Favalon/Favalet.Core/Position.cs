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

        public static implicit operator Position(ValueTuple<int, int> position) =>
            new Position(position.Item1, position.Item2);
    }
}
