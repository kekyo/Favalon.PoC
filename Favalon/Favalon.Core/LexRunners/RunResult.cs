using Favalon.Tokens;

namespace Favalon.LexRunners
{
    internal struct RunResult
    {
        public readonly LexRunner Next;
        public readonly Token? Token0;
        public readonly Token? Token1;

        private RunResult(LexRunner next, Token? token0, Token? token1)
        {
            this.Next = next;
            this.Token0 = token0;
            this.Token1 = token1;
        }

        public static RunResult Empty(LexRunner next) =>
            new RunResult(next, null, null);
        public static RunResult Create(LexRunner next, Token? token0) =>
            new RunResult(next, token0, null);
        public static RunResult Create(LexRunner next, Token? token0, Token? token1) =>
            new RunResult(next, token0, token1);

        public void Deconstruct(out LexRunner next, out Token? token0, out Token? token1)
        {
            next = this.Next;
            token0 = this.Token0;
            token1 = this.Token1;
        }
    }
}
