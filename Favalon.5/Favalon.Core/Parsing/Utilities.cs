﻿// This is part of Favalon project - https://github.com/kekyo/Favalon
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
using System.Runtime.CompilerServices;

namespace Favalon.Parsing
{
    internal static class Utilities
    {
        private static readonly char[] declarableOperators = new[]
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''', '(', ')' */, '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
        };

#line hidden
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnter(char ch) =>
            (ch == '\r') || (ch == '\n');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDeclarableOperator(char ch) =>
            Array.BinarySearch(declarableOperators, ch) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanFeedback(char ch) =>
            !char.IsControl(ch);
#line default
    }
}
