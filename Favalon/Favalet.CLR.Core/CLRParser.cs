using System.Diagnostics;
using Favalet.Parsers;

namespace Favalet
{
    public sealed class CLRParser : Parser
    {
        [DebuggerStepThrough]
        private sealed class Factory : IParseRunnerFactory
        {
            private Factory()
            { }

            public ParseRunner Waiting { get; } = WaitingRunner.Instance;
            public ParseRunner Applying { get; } = ApplyingRunner.Instance;
            
            public static IParseRunnerFactory Instance =
                new Factory();
        }

        [DebuggerStepThrough]
        private CLRParser(IParseRunnerFactory factory) :
            base(factory)
        { }
        
        public new static readonly CLRParser Instance =
            new CLRParser(Factory.Instance);
    }
}