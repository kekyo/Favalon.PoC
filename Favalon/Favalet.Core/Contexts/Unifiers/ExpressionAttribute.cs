using System.Diagnostics;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerStepThrough]
    internal readonly struct ExpressionAttribute
    {
        public readonly bool Forward;
        public readonly bool Fixed;

        private ExpressionAttribute(bool forward, bool @fixed)
        {
            this.Forward = forward;
            this.Fixed = @fixed;
        }

        public ExpressionAttribute Reverse() =>
            new ExpressionAttribute(!this.Forward, this.Fixed);
            
        public ExpressionAttribute ApplyFixed(bool @fixed) =>
            new ExpressionAttribute(this.Forward, this.Fixed || @fixed);

        public override string ToString() =>
            (this.Forward, this.Fixed) switch
            {
                (false, false) => "Backward",
                (false, true) => "Backward,Fixed",
                (true, false) => "Forward",
                (true, true) => "Forward,Fixed",
            };

        public static ExpressionAttribute Create(bool @fixed) =>
            new ExpressionAttribute(true, @fixed);
    }
}
