using Favalon.Internal;
using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalon
{
    public sealed partial class Environment : Context
    {
        private static readonly ManagedDictionary<string, List<BoundTermInformation>> defaultBoundTerms =
            new ManagedDictionary<string, List<BoundTermInformation>>();

        static Environment()
        {
            AddBoundTerm(
                defaultBoundTerms,
                "true",
                BoundTermNotations.Prefix,
                BoundTermAssociatives.LeftToRight,
                null,
                BooleanTerm.True);
            AddBoundTerm(
                defaultBoundTerms,
                "false",
                BoundTermNotations.Prefix,
                BoundTermAssociatives.LeftToRight,
                null,
                BooleanTerm.False);

            TermUtilities.AddBoundTermsFromAssembly(defaultBoundTerms, typeof(object).GetAssembly());

            AddBoundTerm(
                defaultBoundTerms,
                "->",
                BoundTermNotations.Infix,
                BoundTermAssociatives.RightToLeft,
                BoundTermPrecedences.Morphism,
                TermUtilities.LambdaArrowOperator);
        }

        public void AddBoundTermFromMethod(MethodInfo method) =>
            TermUtilities.AddBoundTermFromMethod(boundTerms, method.GetFullName(), method);

        public void AddBoundTermFromType(Type type) =>
            TermUtilities.AddBoundTermFromType(boundTerms, type);

        public void AddBoundTermsFromAssembly(Assembly assembly) =>
            TermUtilities.AddBoundTermsFromAssembly(boundTerms, assembly);

        private Environment(ManagedDictionary<string, List<BoundTermInformation>> boundTerms) : base(boundTerms)
        { }

        public static Environment Create(bool pure = false) =>
            new Environment(pure ? new ManagedDictionary<string, List<BoundTermInformation>>() : defaultBoundTerms.Clone());
    }
}
