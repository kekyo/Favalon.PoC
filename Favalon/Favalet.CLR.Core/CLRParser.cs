using System.Diagnostics;
using Favalet.Parsers;

namespace Favalet
{
    public sealed class CLRParser : Parser
    {
        [DebuggerStepThrough]
        private CLRParser() :
            base(CLRParseRunnerContext.Create)
        { }
        
        public new static readonly CLRParser Instance =
            new CLRParser();
    }
}