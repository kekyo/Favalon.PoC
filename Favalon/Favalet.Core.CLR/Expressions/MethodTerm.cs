﻿////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Contexts;
using System;
using System.Linq;
using System.Reflection;

namespace Favalet.Expressions
{
    public sealed class MethodTerm :
        Term, IConstantTerm, ICallableExpression
    {
        public readonly MethodInfo Method;

        private MethodTerm(MethodInfo method) =>
            this.Method = method;

        public override IExpression HigherOrder =>
            TypeTerm.FromFunction(
                this.Method.ReturnType,
                this.Method.GetParameters().Select(p => p.ParameterType).ToArray());

        object IConstantTerm.Value =>
            this.Method;

        public TryCallResultContains TryCall(
            IReduceContext context, IExpression argument, out IExpression result)
        {
            var reducedArgument = argument.ReduceIfRequired(context);
            if (reducedArgument is IConstantTerm constantArgument)
            {
                // TODO: this (arg0)
                // TODO: multiple arguments
                // TODO: void
                var calledResult = this.Method.Invoke(null, new object[] { constantArgument.Value });

                result = ConstantTerm.From(calledResult);
                return TryCallResultContains.CalledResult;
            }
            else
            {
                result = reducedArgument;
                return TryCallResultContains.InterpretedArgument;
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is MethodTerm method &&
                this.Method.Equals(method.Method);

        public static MethodTerm From(MethodInfo method) =>
            new MethodTerm(method);
        public static MethodTerm From(Type type, string name, params Type[] parameters) =>
            new MethodTerm(type.GetMethod(name, parameters));
    }
}
