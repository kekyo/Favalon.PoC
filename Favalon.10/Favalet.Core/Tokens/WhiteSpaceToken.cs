﻿////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace Favalet.Tokens
{
    public sealed class WhiteSpaceToken :
        Token, IEquatable<WhiteSpaceToken?>
    {
        private WhiteSpaceToken()
        { }

        public override int GetHashCode() =>
            0;

        public bool Equals(WhiteSpaceToken? other) =>
            other != null;

        public override bool Equals(object obj) =>
            this.Equals(obj as WhiteSpaceToken);

        protected override string FormatString() =>
            string.Empty;

        internal static readonly WhiteSpaceToken Instance =
            new WhiteSpaceToken();
    }
}
