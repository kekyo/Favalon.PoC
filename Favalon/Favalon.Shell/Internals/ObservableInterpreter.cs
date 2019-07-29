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
using Favalet.Terms;
using Favalet.Terms.Basis;
using Favalon.IO;
using Favalon.Parsing;
using System;

namespace Favalon.Internals
{
    internal sealed class ObservableInterpreter : IObserver<ParseResult>, IDisposable
    {
        private readonly ObservableParser<InteractiveConsoleHost> parser;
        private readonly IDisposable parserDisposable;

        public ObservableInterpreter(ObservableParser<InteractiveConsoleHost> parser)
        {
            this.parser = parser;
            this.parserDisposable = this.parser.Subscribe(this);

            this.Terrain = Terrain.Create();
        }

        public void Dispose() =>
            this.parserDisposable.Dispose();

        public Terrain Terrain { get; }

        private Term? WriteInfer(Term term)
        {
            parser.InteractiveHost.WriteLine(FeedbackLevels.Information, $"parsed: {term.AnnotatedReadableString}");
            var (inferred, errors) = Terrain.Infer(term, Term.Unspecified);
            foreach (var error in errors)
            {
                parser.InteractiveHost.WriteLine(FeedbackLevels.Error, $"{error}");
            }
            if (inferred != null)
            {
                parser.InteractiveHost.WriteLine(FeedbackLevels.Information, $"inferred: {inferred.AnnotatedReadableString}");
            }
            return inferred;
        }

        void IObserver<ParseResult>.OnNext(ParseResult result)
        {
            var abort = false;
            switch (result)
            {
                case ParseResult(FreeVariableTerm term, _, _) when term.Name == "exit":
                    parser.InteractiveHost.Abort();
                    abort = true;
                    break;

                case ParseResult(Term term, _, TextRange targetTextRange):
                    if (WriteInfer(term) is Term inferred)
                    {
                        var targetTerms = inferred.ExtractTermsByOverlaps(targetTextRange);
                    }
                    break;

                case ParseResult(Term term, _, _):
                    WriteInfer(term);
                    break;

                case ParseResult(_, ParseErrorInformation[] errors2, _):
                    foreach (var error in errors2)
                    {
                        parser.InteractiveHost.WriteLine(FeedbackLevels.Error, $"{error}");
                    }
                    break;
            }

            if (!abort)
            {
                parser.InteractiveHost.Write(FeedbackLevels.Feedback, "Favalon> ");
            }
        }

        void IObserver<ParseResult>.OnCompleted() =>
            parser.InteractiveHost.Write(FeedbackLevels.Information, "Exited from Favalon.");

        void IObserver<ParseResult>.OnError(Exception ex) =>
            parser.InteractiveHost.Write(FeedbackLevels.Error, ex.ToString());

        public static ObservableInterpreter Create(ObservableParser<InteractiveConsoleHost> parser) =>
            new ObservableInterpreter(parser);
    }
}
