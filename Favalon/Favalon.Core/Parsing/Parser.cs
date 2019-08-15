using Favalet;
using Favalet.Terms;
using Favalon.Parsing.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Parsing
{
    internal struct StateContext
    {
        private readonly StringBuilder token;
        private readonly List<ErrorInformation> errors;
        private readonly TextRange textRange;
        private readonly Position position;
        private readonly Term? lastTerm;

        private StateContext(
            StringBuilder token, List<ErrorInformation> errors,
            TextRange textRange, Position position,
            Term? lastTerm)
        {
            this.token = token;
            this.errors = errors;
            this.textRange = textRange;
            this.position = position;
            this.lastTerm = lastTerm;
        }

        public StateContext(TextRange textRange)
        {
            this.token = new StringBuilder();
            this.errors = new List<ErrorInformation>();
            this.textRange = textRange;
            this.position = textRange.Range.First;
            this.lastTerm = null;
        }

        public StateContext Forward() =>
            new StateContext(this.token, this.errors, this.textRange, this.position + 1, this.lastTerm);

        public StateContext AppendTokenCharAndForward(char inch)
        {
            this.token.Append(inch);
            return new StateContext(this.token, this.errors, this.textRange, this.position + 1, this.lastTerm);
        }

        public StateContext RecordError(string details)
        {
            this.errors.Add(ParseErrorInformation.Create(true, details, this.textRange));
            return new StateContext(this.token, this.errors, this.textRange, this.position + 1, this.lastTerm);
        }

        public string ExtractToken()
        {
            var token = this.token.ToString();
            this.token.Clear();
            return token;
        }

        public StateContext CombineTerm(Term? term) =>
            new StateContext(this.token, this.errors, this.textRange, this.position + 1, term);
    }

    public sealed class Parser
    {
        private State state = DetectState.Instance;
        private StateContext stateContext;

        public Parser(TextRange textRange) =>
            stateContext = new StateContext(textRange);

        public async ValueTask ParseAsync(TextReader tr)
        {
            var buffer = new char[4090];

            while (true)
            {
                var read = (tr.Peek() == -1) ?
                    tr.Read(buffer, 0, buffer.Length) :
                    await tr.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (read == 0)
                {
                    break;
                }

                var bufferIndex = 0;
                while (bufferIndex < read)
                {
                    var inch = buffer[bufferIndex++];
                    var (s, sc) = state.Run(inch, stateContext);
                    state = s;
                    stateContext = sc;
                }
            }
        }
    }
}
