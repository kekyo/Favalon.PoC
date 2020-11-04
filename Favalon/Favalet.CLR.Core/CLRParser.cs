using System.Diagnostics;
using Favalet.Parsers;

namespace Favalet
{
    public sealed class CLRParser : Parser
    {
        [DebuggerStepThrough]
        private CLRParser() :
            base(ParseRunnerFactory.Instance)
        { }
        
        public new static readonly CLRParser Instance =
            new CLRParser();
    }
}