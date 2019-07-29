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

namespace Favalon.IO
{
    public struct InteractiveInformation
    {
        public readonly char Character;
        public readonly InteractiveModifiers Modifier;

        private InteractiveInformation(char character, InteractiveModifiers modifier)
        {
            this.Character = character;
            this.Modifier = modifier;
        }

        public override int GetHashCode() =>
            this.Character.GetHashCode() ^ this.Modifier.GetHashCode();

        public bool Equals(InteractiveInformation rhs) =>
            this.Character.Equals(rhs.Character) && this.Modifier.Equals(rhs.Modifier);

        public override bool Equals(object obj) =>
            obj is InteractiveInformation ii ? this.Equals(ii) : false;

        public static InteractiveInformation Create(char character, InteractiveModifiers modifier) =>
            new InteractiveInformation(character, modifier);

        public static bool operator ==(InteractiveInformation lhs, InteractiveInformation rhs) =>
            lhs.Equals(rhs);
        public static bool operator !=(InteractiveInformation lhs, InteractiveInformation rhs) =>
            !lhs.Equals(rhs);
    }
}
