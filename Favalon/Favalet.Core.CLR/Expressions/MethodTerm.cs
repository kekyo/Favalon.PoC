////////////////////////////////////////////////////////////////////////////
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
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Favalet.Expressions
{
    public abstract class MethodTerm :
        Expression, ICallableExpression
    {
        public readonly MethodBase Method;

        private protected MethodTerm(MethodBase method) =>
            this.Method = method;

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

        public override sealed bool Equals(IExpression? rhs) =>
            rhs is MethodTerm method &&
                this.Method.Equals(method.Method);

        public override int GetHashCode() =>
            this.Method.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.UseSimpleLabel ?
                $"{this.Method.GetFullName()}()" :
                context.Format(this, this.Method.GetFullName());

        public static ConstructorTerm From(Type type, params Type[] parameters) =>
            new ConstructorTerm(type.GetDeclaredConstructor(parameters));

        public static ConcreteMethodTerm From(Type type, string name, params Type[] parameters) =>
            new ConcreteMethodTerm(type.GetDeclaredMethod(name, parameters));

        public static MethodTerm From(MethodBase method) =>
            // TODO: instance method
            // TODO: multiple arguments
            // TODO: generic method
            method switch
            {
                ConstructorInfo c => new ConstructorTerm(c),
                MethodInfo m => new ConcreteMethodTerm(m),
                _ => throw new ArgumentException()
            };
    }

    public sealed class ConstructorTerm : MethodTerm
    {
        private readonly ValueLazy<ConstructorInfo, IExpression> higherOrder;

        internal ConstructorTerm(ConstructorInfo constructor) :
            base(constructor) =>
            // It couldn't require higher order informations when calls method.
            this.higherOrder = ValueLazy.Create(
                constructor,
                @constructor =>
                // TODO: multiple arguments
                // TODO: nothing arguments
                // TODO: void returns
                FunctionDeclaredExpression.From(
                    TypeTerm.From(@constructor.GetParameters().Select(p => p.ParameterType).Last()),
                    TypeTerm.From(@constructor.DeclaringType)));

        public override IExpression HigherOrder =>
            this.higherOrder.Value;
    }

    public sealed class ConcreteMethodTerm : MethodTerm
    {
        private readonly ValueLazy<MethodInfo, IExpression> higherOrder;

        internal ConcreteMethodTerm(MethodInfo method) :
            base(method) =>
            // It couldn't require higher order informations when calls method.
            this.higherOrder = ValueLazy.Create(
                method,
                @method =>
                // TODO: multiple arguments
                // TODO: nothing arguments
                FunctionDeclaredExpression.From(
                    TypeTerm.From(@method.GetParameters().Select(p => p.ParameterType).Last()),
                    TypeTerm.From(@method.ReturnType)));

        public override IExpression HigherOrder =>
            this.higherOrder.Value;
    }
}
