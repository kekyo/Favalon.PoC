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

using Favalon.IO;
using Favalon.Parsing.States;
using System;

namespace Favalon.Parsing
{
    public abstract class InteractiveParser : ObservableBase<ParseResult>
    {
        internal InteractiveParser() { }

        public static InteractiveParser<T> Create<T>(T interactiveHost)
            where T : InteractiveHost =>
            new InteractiveParser<T>(interactiveHost);
    }

    public sealed class InteractiveParser<THost> :
        InteractiveParser, IObserver<InteractiveInformation>
        where THost : InteractiveHost
    {
        private readonly StateContext stateContext;
        private readonly IDisposable interactiveHostDisposable;
        private State currentState = DetectState.Instance;

        internal InteractiveParser(THost interactiveHost)
        {
            this.Host = interactiveHost;
            stateContext = new StateContext(this.Host.TextRange);
            interactiveHostDisposable = this.Host.Subscribe(this);
        }

        public void Dispose() =>
            interactiveHostDisposable.Dispose();

        public THost Host { get; private set; }

        void IObserver<InteractiveInformation>.OnNext(InteractiveInformation value)
        {
            // Control-Space: suggesting
            if ((value.Character == ' ') && (value.Modifier == InteractiveModifiers.Control))
            {
                // Peek partially terms
                if (currentState.PeekResult(stateContext) is ParseResult result)
                {
                    // Push (Contans current position)
                    base.OnNext(result);
                }

                return;
            }

            // Backspace
            if (value.Character == '\b')
            {
                stateContext.BackwardToken();
                this.Host.Write(value.Character);
                return;
            }

            ///////////////////////////////////////////////////////////////

            // Examine state
            var newState = currentState.Run(value, stateContext);

            // Echo back character
            if (Utilities.CanFeedback(value.Character) && (value.Modifier == InteractiveModifiers.None))
            {
                this.Host.Write(value.Character);
            }

            // Detedted enter sequence (CR/LF)
            var isEntered = (currentState, newState) switch
            {
                (AfterEnterState _, DetectState _) => false,
                (_, AfterEnterState _) => true,
                _ => false
            };

            if (isEntered)
            {
                // Echo back enter
                this.Host.WriteLine();

                // Extract result by inputted 1 line
                if (stateContext.ExtractResult() is ParseResult result)
                {
                    // Push
                    base.OnNext(result);
                }
            }

            // Update next state
            currentState = newState;
        }

        void IObserver<InteractiveInformation>.OnCompleted()
        {
            // Finalize this state
            currentState.Finalize(stateContext);

            // Extract result by last 1 line
            if (stateContext.ExtractResult() is ParseResult result)
            {
                // Push
                base.OnNext(result);
            }

            // Completed Rx
            base.OnCompleted();
        }

        void IObserver<InteractiveInformation>.OnError(Exception ex) =>
            base.OnError(ex);
    }
}
