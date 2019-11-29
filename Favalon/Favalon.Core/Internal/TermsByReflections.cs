using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Internal
{
    partial class ReflectionUtilities
    {
        public static void InternalAddBoundTermFromType(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Type type)
        {
            // TODO:
            //   1. construct nested term from multiple parameter methods.
            //   2. construct specialized term from instance method (arg0 is this parameter)
            //   3. construct specialized term from empty parameter methods (uses unit?)
            //   4. construct specialized term from constructors.
            //   5. construct specialized term for cast operator.
            //   6. construct specialized term from operator methods.

            var identity = type.GetFullName();
            Context.InternalAddBoundTerm(boundTerms,
                identity,
                BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                new TypeTerm(type));
        }

        public static void InternalAddBoundTermFromMethod(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            MethodInfo method)
        {
            // TODO:
            //   1. construct nested term from multiple parameter methods.
            //   2. construct specialized term from instance method (arg0 is this parameter)
            //   3. construct specialized term from empty parameter methods (uses unit?)
            //   4. construct specialized term from constructors.
            //   5. construct specialized term for cast operator.
            //   6. construct specialized term from operator methods.

            var identity = method.GetFullName();
            Context.InternalAddBoundTerm(boundTerms,
                identity,
                BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                new MethodTerm(method));
        }

        public static void InternalAddBoundTermsFromAssembly(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().
                Where(type => (type.IsPublic() || type.IsNestedPublic()) && !type.IsGenericType()))
            {
                InternalAddBoundTermFromType(boundTerms, type);

                foreach (var method in type.GetMethods().
                    Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod && method.GetParameters().Length == 1).
                    GroupBy(method => method.GetFullName()).
                    SelectMany(g => g))
                {
                    InternalAddBoundTermFromMethod(boundTerms, method);
                }
            }
        }
    }
}
