////////////////////////////////////////////////////////////////////////////
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

namespace Favalet.Expressions
{
    [Flags]
    public enum BoundTermAttributes
    {
        Prefix = 0x00,
        Infix = 0x01,
        LeftToRight = 0x00,
        RightToLeft = 0x02,
    }

    public enum BoundTermPrecedences
    {
        Lowest = 0,
        Method = 100,
        Function = 1000,
        Morphism = 3000,
        ArithmericAddition = 5000,
        ArithmericMultiplication = 6000,
    }
}
