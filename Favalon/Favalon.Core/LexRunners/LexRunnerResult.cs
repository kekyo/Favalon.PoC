﻿using Favalon.Tokens;
using System.Runtime.CompilerServices;

namespace Favalon.LexRunners
{
    internal struct LexRunnerResult
    {
        public readonly LexRunner Next;
        public readonly Token? Token0;
        public readonly Token? Token1;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private LexRunnerResult(LexRunner next, Token? token0, Token? token1)
        {
            this.Next = next;
            this.Token0 = token0;
            this.Token1 = token1;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static LexRunnerResult Empty(LexRunner next) =>
            new LexRunnerResult(next, null, null);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static LexRunnerResult Create(LexRunner next, Token? token0) =>
            new LexRunnerResult(next, token0, null);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static LexRunnerResult Create(LexRunner next, Token? token0, Token? token1) =>
            new LexRunnerResult(next, token0, token1);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out LexRunner next, out Token? token0, out Token? token1)
        {
            next = this.Next;
            token0 = this.Token0;
            token1 = this.Token1;
        }
    }
}
