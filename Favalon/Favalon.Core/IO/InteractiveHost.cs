﻿// This is part of Favalon project - https://github.com/kekyo/Favalon
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
using System;

namespace Favalon.IO
{
    public enum FeedbackLevels
    {
        Feedback,
        Information,
        Warning,
        Error
    }

    public abstract class InteractiveHost :
        ObservableBase<InteractiveInformation>
    {
        protected InteractiveHost(TextRange textRange) =>
            this.TextRange = textRange;

        public TextRange TextRange { get; }

        public abstract void Write(FeedbackLevels level, char ch);

        public virtual void Write(FeedbackLevels level, string text)
        {
            foreach (var ch in text)
            {
                this.Write(level, ch);
            }
        }

        public virtual void WriteLine(FeedbackLevels level, string text)
        {
            this.Write(level, text);
            this.Write(level, Environment.NewLine);
        }

        public virtual void WriteLine(FeedbackLevels level) =>
            this.Write(level, Environment.NewLine);
    }
}
