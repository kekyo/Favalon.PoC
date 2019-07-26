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
    public sealed class ObservableParser :
        ObservableBase<ParseResult>, IObserver<InteractiveInformation>
    {
        private readonly StateContext stateContext;
        private readonly InteractiveHost interactiveHost;
        private readonly IDisposable interactiveHostDisposable;
        private State currentState = DetectState.Instance;

        public ObservableParser(InteractiveHost interactiveHost)
        {
            this.interactiveHost = interactiveHost;
            stateContext = new StateContext(this.interactiveHost.TextRange);
            interactiveHostDisposable = this.interactiveHost.Subscribe(this);
        }

        public void Dispose() =>
            interactiveHostDisposable.Dispose();

        void IObserver<InteractiveInformation>.OnNext(InteractiveInformation value)
        {
            var newState = currentState.Run(value, stateContext);

            if (!char.IsControl(value.Character) && (value.Modifier == InteractiveModifiers.None))
            {
                interactiveHost.Write(value.Character);
            }

            if (!(currentState is DetectState) && newState is DetectState && State.IsTokenSeparator(value.Character))
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

        public static ObservableParser Create(InteractiveHost interactiveHost) =>
            new ObservableParser(interactiveHost);
    }
}
