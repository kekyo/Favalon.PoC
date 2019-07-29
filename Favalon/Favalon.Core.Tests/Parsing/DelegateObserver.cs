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

using System;
using System.Collections.Generic;

namespace Favalon.Parsing
{
    public sealed class DelegateObserver<T> : IObserver<T>
    {
        private readonly Queue<Action<T>> onNexts;

        private DelegateObserver(Action<T>[] onNexts) =>
            this.onNexts = new Queue<Action<T>>(onNexts);

        public void OnNext(T value) =>
            onNexts.Dequeue()(value);

        public void OnCompleted()
        {
        }

        public void OnError(Exception ex)
        {
        }

        public static DelegateObserver<T> Create(params Action<T>[] onNexts) =>
            new DelegateObserver<T>(onNexts);
    }
}
