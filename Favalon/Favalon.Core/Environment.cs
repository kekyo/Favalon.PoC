using Favalon.Internal;
using Favalon.Terms;
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
            ReflectionUtilities.InternalAddBoundTermsFromAssembly(defaultBoundTerms, typeof(object).GetAssembly());

            // operator arrow (lambda constructor)
            // -> a b
            // --------------
            // f  a b
            // ((f:'1->'2 a:'1):'2 b:'3):'4
            // ((f:'1->'2 a:'1):'3->'4 b:'3):'4
            // ((f:'1->'3->'4 a:'1):'3->'4 b:'3):'4
            InternalAddBoundTerm(
                defaultBoundTerms,
                "->",
                BoundTermNotations.Infix,
                BoundTermAssociatives.RightToLeft,
                BoundTermPrecedences.Morphism,
                // f:'1->'3->'4
                new DelegationTerm<IdentityTerm>(
                    "->", "a",  // a:'1
                    (ic, a) =>
                        // '3->'4
                        new DelegationTerm<Term>(
                            $"Closure(-> {a})", "b",  // b:'3
                            (oc, b) =>
                                new FunctionTerm(((IdentityTerm)a.VisitReduce(ic)).ToBoundIdentity(), b.VisitReduce(oc)))));
        }

        public void AddBoundTermFromMethod(MethodInfo method) =>
            ReflectionUtilities.InternalAddBoundTermFromMethod(boundTerms, method);

        public void AddBoundTermsFromAssembly(Assembly assembly) =>
            ReflectionUtilities.InternalAddBoundTermsFromAssembly(boundTerms, assembly);

        private Environment(ManagedDictionary<string, List<BoundTermInformation>> boundTerms) : base(boundTerms)
        { }

        public static Environment Create(bool pure = false) =>
            new Environment(pure ? new ManagedDictionary<string, List<BoundTermInformation>>() : defaultBoundTerms.Clone());
    }
}
