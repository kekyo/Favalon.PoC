using System.Diagnostics;
using Favalet.Tokens;

namespace Favalet.Parsers
{
    [DebuggerStepThrough]
    internal sealed class CLRParseRunnerContext : ParseRunnerContext
    {
        public NumericalSignToken? PreSignToken;

        private CLRParseRunnerContext()
        { }
        
        public override ParseRunner Waiting =>
            CLRWaitingRunner.Instance;
        public override ParseRunner Applying =>
            CLRApplyingRunner.Instance;

        public new static CLRParseRunnerContext Create() =>
            new CLRParseRunnerContext();
    }
}