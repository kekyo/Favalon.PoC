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
    public sealed class ObservableParser<T> :
        ObservableBase<ParseResult>, IObserver<InteractiveInformation>
        where T : InteractiveHost
    {
        private readonly StateContext stateContext;
        private readonly IDisposable interactiveHostDisposable;
        private State currentState = DetectState.Instance;

        public ObservableParser(T interactiveHost)
        {
            this.InteractiveHost = interactiveHost;
            stateContext = new StateContext(this.InteractiveHost.TextRange);
            interactiveHostDisposable = this.InteractiveHost.Subscribe(this);
        }

        public void Dispose() =>
            interactiveHostDisposable.Dispose();

        public T InteractiveHost { get; private set; }

        void IObserver<InteractiveInformation>.OnNext(InteractiveInformation value)
        {
            if ((value.Character == ' ') && (value.Modifier == InteractiveModifiers.Control))
            {
                if (currentState.PeekResult(stateContext) is ParseResult result)
                {
                    base.OnNext(result);
                }
            }

            var newState = currentState.Run(value, stateContext);

            if (Utilities.CanFeedback(value.Character) && (value.Modifier == InteractiveModifiers.None))
            {
                this.InteractiveHost.Write(value.Character);
            }

            var isEntered = (currentState, newState) switch
            {
                (AfterEnterState _, DetectState _) => false,
                (_, AfterEnterState _) => true,
                _ => false
            };
            if (isEntered)
            {
                if (stateContext.ExtractResult() is ParseResult result)
                {
                    base.OnNext(result);
                }
            }

            currentState = newState;
        }

        void IObserver<InteractiveInformation>.OnCompleted()
        {
            currentState.Finalize(stateContext);

            if (stateContext.ExtractResult() is ParseResult result)
            {
                base.OnNext(result);
            }

            base.OnCompleted();
        }

        void IObserver<InteractiveInformation>.OnError(Exception ex) =>
            base.OnError(ex);

        public static ObservableParser<T> Create(T interactiveHost) =>
            new ObservableParser<T>(interactiveHost);
    }
}
