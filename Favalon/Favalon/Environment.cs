using Favalon.Expressions;
using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    public sealed class Environment
    {
        private sealed class OverallScope
        {
            private int placeholderIndex;

            public int AssignIndex() =>
                placeholderIndex++;
        }

        private static readonly IReadOnlyList<Term> emptyTerms = new Term[0];

        private readonly OverallScope overallScope;
        private readonly ImmutableDictionary<string, ImmutableList<Term>> boundTerms;

        private Environment(OverallScope overallScope, ImmutableDictionary<string, ImmutableList<Term>> boundTerms)
        {
            this.overallScope = overallScope;
            this.boundTerms = boundTerms;
        }

        public IReadOnlyDictionary<string, ImmutableList<Term>> BoundTerms =>
            boundTerms;

        public Environment Bind(string name, Term body) =>
            new Environment(
                overallScope,
                boundTerms.TryGetValue(name, out var terms) ?
                    boundTerms.SetItem(name, terms.Add(body)) :
                    boundTerms.Add(name, ImmutableList<Term>.Empty.Add(body)));

        internal IReadOnlyList<Term> Lookup(string name) =>
            boundTerms.TryGetValue(name, out var body) ? body : emptyTerms;

        internal Placeholder CreatePlaceholder(Term higherOrder) =>
            new Placeholder(overallScope.AssignIndex(), higherOrder);

        public Expression Infer(Term term)
        {
            var inferred = term.VisitInfer(this);
            return inferred;
        }

        public static Environment Create()
        {
            var overallScope = new OverallScope();
            var boundTerms = ImmutableDictionary<string, ImmutableList<Term>>.Empty;

            //var t0 = environment.CreatePlaceholder(Unspecified.Instance);
            //var t1 = environment.CreatePlaceholder(Unspecified.Instance);
            //boundTerms.Add(
            //    "->",
            //    Factories.Function(Factories.Function(Factories.Function(t0, t1), t0), t1));

            var types = typeof(object).GetTypeInfo().Assembly.
                DefinedTypes.
                Where(type =>
                    type.IsPublic && (type.IsClass || type.IsValueType) &&
                    (type.DeclaringType == null) &&
                    !type.IsGenericType).
                ToArray();

            foreach (var type in types)
            {
                var typeSymbol = new TypeSymbol(type);

                boundTerms = boundTerms.Add(typeSymbol.PrintableName, ImmutableList<Term>.Empty.Add(typeSymbol));
            }

            foreach (var (constructor, _) in types.
                SelectMany(type => type.DeclaredConstructors.
                    Where(constructor => constructor.IsPublic && !constructor.IsStatic).
                    Select(constructor => (constructor, parameters: constructor.GetParameters())).
                    OrderBy(entry => entry.parameters.Length).
                    ThenBy(entry => entry.parameters.
                        Sum(parameter => (parameter.ParameterType.GetTypeInfo().IsPrimitive || parameter.ParameterType == typeof(string)) ?
                            0 :
                            parameter.Position * 2))))
            {
                var methodSymbol = new MethodSymbol(constructor);

                boundTerms = boundTerms.TryGetValue(methodSymbol.PrintableName, out var terms) ?
                    boundTerms.SetItem(methodSymbol.PrintableName, terms.Add(methodSymbol)) :
                    boundTerms.Add(methodSymbol.PrintableName, ImmutableList<Term>.Empty.Add(methodSymbol));
            }

            foreach (var (method, _) in types.
                SelectMany(type => type.DeclaredMethods.
                    Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod).
                    Select(method => (method, parameters:method.GetParameters())).
                    OrderBy(entry => entry.parameters.Length).
                    ThenBy(entry => entry.parameters.
                        Sum(parameter => (parameter.ParameterType.GetTypeInfo().IsPrimitive || parameter.ParameterType == typeof(string)) ?
                            0 :
                            parameter.Position * 2))))
            {
                var methodSymbol = new MethodSymbol(method);

                boundTerms = boundTerms.TryGetValue(methodSymbol.PrintableName, out var terms) ?
                    boundTerms.SetItem(methodSymbol.PrintableName, terms.Add(methodSymbol)) :
                    boundTerms.Add(methodSymbol.PrintableName, ImmutableList<Term>.Empty.Add(methodSymbol));
            }

            foreach (var path in Directory.EnumerateFiles(
                @"C:\Program Files\Git\usr\bin", "*.exe", SearchOption.TopDirectoryOnly))
            {
                var executableSymbol = new ExecutableSymbol(path);

                boundTerms = boundTerms.TryGetValue(executableSymbol.PrintableName, out var terms) ?
                    boundTerms.SetItem(executableSymbol.PrintableName, terms.Add(executableSymbol)) :
                    boundTerms.Add(executableSymbol.PrintableName, ImmutableList<Term>.Empty.Add(executableSymbol));
            }

            var environment = new Environment(overallScope, boundTerms);
            return environment;
        }
    }
}
