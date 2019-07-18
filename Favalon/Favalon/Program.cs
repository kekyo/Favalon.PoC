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
using System.Threading.Tasks;

namespace Favalon
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Favalet.Environment.Create();
            var parser = Parser.Create();

            for (var line = 0; line < int.MaxValue; line++)
            {
                await Console.Out.WriteAsync("Favalon> ");

                var text = await Console.In.ReadLineAsync();
                if (parser.Append(text, line) is Expression expression)
                {
                    await Console.Out.WriteLineAsync($"parsed: {expression}");

                    var inferred = environment.Infer(expression);
                    await Console.Out.WriteLineAsync($"inferred: {inferred}");
                }
            }
        }
    }
}
