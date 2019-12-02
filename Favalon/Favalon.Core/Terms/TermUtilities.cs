using Favalon.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms
{
    internal static class TermUtilities
    {
        public static readonly Term BooleanAndOperator =
            new DelegationTerm<Term>(
                "lhs",
                (ic, lhs) =>
                    new DelegationTerm<Term>(
                        "rhs",
                        (oc, rhs) => BooleanTerm.FromConstant(((BooleanTerm)lhs.VisitReduce(ic)).Value && ((BooleanTerm)rhs.VisitReduce(oc)).Value)));
        public static readonly Term BooleanOrOperator =
            new DelegationTerm<Term>(
                "lhs",
                (ic, lhs) =>
                    new DelegationTerm<Term>(
                        "rhs",
                        (oc, rhs) => BooleanTerm.FromConstant(((BooleanTerm)lhs.VisitReduce(ic)).Value || ((BooleanTerm)rhs.VisitReduce(oc)).Value)));

        // operator arrow (lambda calculus)
        // -> a b
        // --------------
        // f  a b
        // ((f:'1->'2 a:'1):'2 b:'3):'4
        // ((f:'1->'2 a:'1):'3->'4 b:'3):'4
        // ((f:'1->'3->'4 a:'1):'3->'4 b:'3):'4
        // f:'1->'3->'4
        public static readonly Term LambdaArrowOperator =
            new DelegationTerm<IdentityTerm>(
                "parameter",  // a:'1
                (ic, parameter) =>
                    // '3->'4
                    new DelegationTerm<Term>(
                        "body",  // b:'3
                        (oc, body) =>
                            new FunctionTerm(((IdentityTerm)parameter.VisitReduce(ic)).ToBoundIdentity(), body.VisitReduce(oc))));
        
        public static Term CreateTypeConstructorTerm(Type gtd) =>
            new DelegationTerm<Term>(
                gtd.GetGenericArguments()[0].GetFullName(),
                (context, a) => new TypeTerm(gtd.MakeGenericType(((TypeTerm)a.VisitReduce(context)).Type)));

        public static void AddBoundTermFromMethod(
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

            Context.AddBoundTerm(boundTerms,
                identity,
                BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                new MethodTerm(method));
        }

        public static void AddBoundTermFromType(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Type type)
        {
            var identity = type.GetFullName(false);

            // The type
            //Context.AddBoundTerm(boundTerms,
            //    identity,
            //    BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
            //    new TypeTerm(type));

            // Type constructor
            if (type.IsGenericTypeDefinition() && type.GetGenericArguments() is Type[] types && types.Length == 1)
            {
                Context.AddBoundTerm(
                    boundTerms,
                    identity,
                    BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, null,
                    CreateTypeConstructorTerm(type));
            }

            // Value constructors
            foreach (var constructor in type.GetConstructors().
                Where(constructor => constructor.IsPublic && !constructor.IsStatic && !constructor.IsGenericMethod && constructor.GetParameters().Length == 1))
            {
                AddBoundTermFromMethod(boundTerms, identity, constructor);
            }

            // Static methods
            foreach (var method in type.GetMethods().
                Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod && method.GetParameters().Length == 1))
            {
                AddBoundTermFromMethod(boundTerms, method.GetFullName(false), method);
            }
        }

        public static void AddBoundTermsFromAssembly(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().
                Where(type => type.IsPublic() || type.IsNestedPublic()))
            {
                AddBoundTermFromType(boundTerms, type);
            }
        }
    }
}
