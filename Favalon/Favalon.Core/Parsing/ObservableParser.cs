// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Favalet;
using Favalon.IO;
using Favalon.Parsing.States;
using System;
using System.Text;

namespace Favalon.Parsing
{
    public sealed class ObservableParser :
        ObservableBase<ParseResult>, IObserver<char>, IObserver<string>, IObserver<InteractiveInformation>
    {
        private readonly TextRange textRange;
        private IDisposable? sourceDisposable;
        private readonly StringBuilder lineBuffer = new StringBuilder();
        private Position startPosition;
        private int columnPosition;
        private char lastCh;

        private ObservableParser(TextRange textRange)
        {
            this.textRange = textRange;
            this.startPosition = textRange.Range.First;
            this.columnPosition = textRange.Range.First.Column;
        }

        private StateContext RunStateMachine()
        {
            var stateContext = new StateContext(
                textRange.Subtract(startPosition, (startPosition.Line, columnPosition)));

            var state = DetectState.Instance;
            for (var index = 0; index < lineBuffer.Length; index++)
            {
                state = state.Run(lineBuffer[index], stateContext);
            }

            state.Finalize(stateContext);

            return stateContext;
        }

        private void OnNext(char inch)
        {
            switch ((inch, lastCh))
            {
                case ('\u0000', _):
                    break;
                case ('\u0008', _):  // BS
                    if (columnPosition > startPosition.Column)
                    {
                        columnPosition--;
                        lineBuffer.Remove(columnPosition - startPosition.Column, 1);
                    }
                    break;
                case ('\u007f', _):  // DEL
                    if (columnPosition < (startPosition.Column + lineBuffer.Length))
                    {
                        lineBuffer.Remove(columnPosition - startPosition.Column, 1);
                    }
                    break;
                case ('\u000a', '\u000d'):  // CR-LF (ignore)
                    break;
                case ('\u000d', _):  // CR
                case ('\u000a', _):  // LF
                    var stateContext = this.RunStateMachine();
                    if (stateContext.ExtractResult() is ParseResult result)
                    {
                        base.OnNext(result);
                    }

                    startPosition = startPosition.MoveLine(1);
                    columnPosition = startPosition.Column;
                    lineBuffer.Clear();
                    break;
                default:
                    lineBuffer.Insert(columnPosition, inch);
                    columnPosition++;
                    break;
            }

            lastCh = inch;
        }

        void IObserver<char>.OnNext(char inch) =>
            this.OnNext(inch);

        void IObserver<char>.OnCompleted() =>
            sourceDisposable!.Dispose();

        void IObserver<char>.OnError(Exception ex) =>
            base.OnError(ex);

        void IObserver<string>.OnNext(string text)
        {
            foreach (var inch in text)
            {
                this.OnNext(inch);
            }
        }

        void IObserver<string>.OnCompleted() =>
            sourceDisposable!.Dispose();

        void IObserver<string>.OnError(Exception ex) =>
            base.OnError(ex);

        public void OnNext(InteractiveInformation value)
        {
            if (value.Modifier.HasFlag(InteractiveModifiers.Control) && (value.Character == ' '))
            {
                var stateContext = this.RunStateMachine();
                if (stateContext.ExtractResult() is ParseResult result)
                {
                    base.OnNext(result);
                }

                startPosition = startPosition.MoveLine(1);
                lineBuffer.Clear();
            }
            else
            {
                this.OnNext(value.Character);
            }
        }

        void IObserver<InteractiveInformation>.OnCompleted() =>
            sourceDisposable!.Dispose();

        void IObserver<InteractiveInformation>.OnError(Exception ex) =>
            base.OnError(ex);

        public static ObservableParser Create(IObservable<char> source, TextRange textRange)
        {
            var parser = new ObservableParser(textRange);
            parser.sourceDisposable = source.Subscribe(parser);
            return parser;
        }

        public static ObservableParser Create(IObservable<string> source, TextRange textRange)
        {
            var parser = new ObservableParser(textRange);
            parser.sourceDisposable = source.Subscribe(parser);
            return parser;
        }

        public static ObservableParser Create(IObservable<InteractiveInformation> source, TextRange textRange)
        {
            var parser = new ObservableParser(textRange);
            parser.sourceDisposable = source.Subscribe(parser);
            return parser;
        }
    }
}
