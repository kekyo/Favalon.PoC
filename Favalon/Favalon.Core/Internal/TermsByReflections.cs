using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Favalon.Internal
{
    partial class ReflectionUtilities
    {
        public static void InternalAddBoundTermFromMethod(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            string identity,
            MethodBase method)
        {
            Debug.Assert(!method.IsGenericMethod);

            // TODO:
            //   1. construct nested term from multiple parameter methods.
            //   2. construct specialized term from instance method (arg0 is this parameter)
            //   3. construct specialized term from empty parameter methods (uses unit?)
            //   4. construct specialized term from constructors.
            //   5. construct specialized term for cast operator.
            //   6. construct specialized term from operator methods.

            Context.InternalAddBoundTerm(boundTerms,
                identity,
                BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                new MethodTerm(method));
        }

        public static void InternalAddBoundTermFromType(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Type type)
        {
            Context.InternalAddBoundTerm(boundTerms,
                type.GetFullName(false),
                BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                new TypeTerm(type));

            foreach (var constructor in type.GetConstructors().
                Where(constructor => constructor.IsPublic && !constructor.IsStatic && !constructor.IsGenericMethod && constructor.GetParameters().Length == 1).
                GroupBy(constructor => constructor.GetFullName()).
                SelectMany(g => g))
            {
                InternalAddBoundTermFromMethod(boundTerms, constructor.DeclaringType.GetFullName(false), constructor);
            }

            foreach (var method in type.GetMethods().
                Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod && method.GetParameters().Length == 1).
                GroupBy(method => method.GetFullName()).
                SelectMany(g => g))
            {
                InternalAddBoundTermFromMethod(boundTerms, method.GetFullName(false), method);
            }
        }

        public static void InternalAddBoundTermsFromAssembly(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().
                Where(type => type.IsPublic() || type.IsNestedPublic()))
            {
                InternalAddBoundTermFromType(boundTerms, type);
            }
        }
    }
}
