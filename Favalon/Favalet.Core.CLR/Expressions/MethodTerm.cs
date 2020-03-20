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
using Favalet.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Favalet.Expressions
{
    public sealed class MethodTerm :
        Expression, IConstantTerm, ICallableExpression
    {
        public readonly MethodBase Method;
        private readonly Lazy<IExpression> higherOrder;

        private MethodTerm(MethodBase method)
        {
            this.Method = method;

            // It couldn't require higher order informations when calls method.
            this.higherOrder = Lazy.Create(() =>
                // TODO: multiple arguments
                // TODO: nothing arguments
                // TODO: void returns
                FunctionDeclaredExpression.From(
                    TypeTerm.From(this.Method.GetParameters().Select(p => p.ParameterType).Last()),
                    TypeTerm.From(this.Method is MethodInfo m ? m.ReturnType : this.Method.DeclaringType)));
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        object IConstantTerm.Value =>
            this.Method;

        public TryCallResultContains TryCall(
            IReduceContext context, IExpression argument, out IExpression result)
        {
            var reducedArgument = argument.ReduceIfRequired(context);
            if (reducedArgument is IConstantTerm constantArgument)
            {
                // TODO: this (arg0, instance method)
                // TODO: multiple arguments
                // TODO: void returns
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

        public override int GetHashCode() =>
            this.Method.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, this.Method.GetFullName());

        public static MethodTerm From(Type type, string name, params Type[] parameters) =>
            new MethodTerm(type.GetMethod(name, parameters));

        public static MethodTerm From(MethodBase method) =>
            // TODO: instance method
            // TODO: multiple arguments
            // TODO: generic method
            new MethodTerm(method);
    }
}
