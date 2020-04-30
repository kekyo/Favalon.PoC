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

using Favalet.Expressions;
using Favalet.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Favalet.Contexts
{
    public static class TypeEnvironmentExtension
    {
        public static TTypeEnvironment MutableBindConstructor<TTypeEnvironment>(
            this TTypeEnvironment environment, ConstructorInfo constructor)
            where TTypeEnvironment : ITypeEnvironment
        {
            var identity = constructor.GetFullName(false);
            environment.MutableBind(identity, MethodTerm.From(constructor));

            return environment;
        }

        public static TTypeEnvironment MutableBindMethod<TTypeEnvironment>(
            this TTypeEnvironment environment, MethodInfo method)
            where TTypeEnvironment : ITypeEnvironment
        {
            var identity = method.GetFullName(false);
            environment.MutableBind(identity, MethodTerm.From(method));

            return environment;
        }

        public static TTypeEnvironment MutableBindType<TTypeEnvironment>(
            this TTypeEnvironment environment, Type type)
            where TTypeEnvironment : ITypeEnvironment
        {
            var identity = type.GetFullName(false);
            environment.MutableBind(identity, TypeTerm.From(type));
            environment.MutableBind(identity, ConstantTerm.From(type));

            foreach (var constructor in type.GetDeclaredConstructors().
                Where(c => c.IsPublic && !c.IsPrivate && !c.IsStatic &&
                    (c.GetParameters().Length == 1)))  // TODO: multiple/nothing arguments
            {
                MutableBindConstructor(environment, constructor);
            }

            foreach (var method in type.GetDeclaredMethods().
                Where(m => m.IsPublic && !m.IsPrivate &&
                    (m.GetParameters().Length == 1)))  // TODO: multiple/nothing arguments
            {
                MutableBindMethod(environment, method);
            }

            // TODO: properties?
            // TODO: events?

            return environment;
        }

        public static TTypeEnvironment MutableBindTypes<TTypeEnvironment>(
            this TTypeEnvironment environment, Assembly assembly)
            where TTypeEnvironment : ITypeEnvironment
        {
            foreach (var type in assembly.GetTypes().
                Where(t => t.IsPublic() && (t.IsClass() || t.IsValueType() || t.IsInterface())))
            {
                MutableBindType(environment, type);
            }
            return environment;
        }
    }
}
