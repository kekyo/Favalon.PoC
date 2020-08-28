using System.Diagnostics;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerStepThrough]
    internal readonly struct Attribute
    {
        public readonly bool Forward;
        public readonly bool Fixed;

        private Attribute(bool forward, bool @fixed)
        {
            this.Forward = forward;
            this.Fixed = @fixed;
        }

        public Attribute Reverse() =>
            new Attribute(!this.Forward, this.Fixed);
            
        public Attribute ApplyFixed(bool @fixed) =>
            new Attribute(this.Forward, this.Fixed || @fixed);

        public override string ToString() =>
            (this.Forward, this.Fixed) switch
            {
                (false, false) => "Backward",
                (false, true) => "Backward,Fixed",
                (true, false) => "Forward",
                (true, true) => "Forward,Fixed",
            };

        public static Attribute Create(bool @fixed) =>
            new Attribute(true, @fixed);
    }
}
