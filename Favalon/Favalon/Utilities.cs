using Favalon.Terms;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    internal static class Utilities
    {
        public static readonly ImmutableDictionary<string, ImmutableList<Term>> EmptyBoundTerms =
            ImmutableDictionary<string, ImmutableList<Term>>.Empty;

        public static ImmutableDictionary<string, ImmutableList<Term>> AddExecutables(
            this ImmutableDictionary<string, ImmutableList<Term>> boundTerms,
            string targetDirectory,
            string searchPattern)
        {
            foreach (var path in Directory.EnumerateFiles(
                targetDirectory, searchPattern, SearchOption.TopDirectoryOnly))
            {
                var executableSymbol = new ExecutableSymbol(path);

                boundTerms = boundTerms.TryGetValue(executableSymbol.PrintableName, out var terms) ?
                    boundTerms.SetItem(executableSymbol.PrintableName, terms.Add(executableSymbol)) :
                    boundTerms.Add(executableSymbol.PrintableName, ImmutableList<Term>.Empty.Add(executableSymbol));
            }

            return boundTerms;
        }

        public static ImmutableDictionary<string, ImmutableList<Term>> AddAssemblyMembers(
            this ImmutableDictionary<string, ImmutableList<Term>> boundTerms,
            Assembly assembly)
        {
            var types = assembly.
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
                    Select(method => (method, parameters: method.GetParameters())).
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

            return boundTerms;
        }

    }
}
