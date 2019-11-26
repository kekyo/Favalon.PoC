using Favalon.Internal;
using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed partial class Environment : Context
    {
        private static readonly ManagedDictionary<string, List<BoundTermInformation>> defaultBoundTerms =
            new ManagedDictionary<string, List<BoundTermInformation>>();

        static Environment()
        {
            foreach (var entry in
                typeof(object).GetAssembly().
                EnumerableAllPublicStaticMethods().
                Where(method => method.GetParameters().Length == 1).
                GroupBy(method => method.GetFullName()).
                SelectMany(g => g.Select(method => new { g.Key, method })))
            {
                // TODO:
                //   1. construct nested term from multiple parameter methods.
                //   2. construct specialized term from instance method (arg0 is this parameter)
                //   3. construct specialized term from empty parameter methods (uses unit?)
                //   4. construct specialized term from constructors.
                //   5. construct specialized term from operator methods.

                AddBoundTerm(
                    defaultBoundTerms,
                    entry.Key,
                    BoundTermNotations.Prefix, BoundTermAssociatives.LeftToRight, BoundTermPrecedences.Method,
                    new MethodTerm(entry.method));
            }

            // operator arrow (lambda constructor)
            // -> a b
            // --------------
            // f  a b
            // ((f:'1->'2 a:'1):'2 b:'3):'4
            // ((f:'1->'2 a:'1):'3->'4 b:'3):'4
            // ((f:'1->'3->'4 a:'1):'3->'4 b:'3):'4
            AddBoundTerm(
                defaultBoundTerms,
                "->",
                BoundTermNotations.Infix,
                BoundTermAssociatives.RightToLeft,
                BoundTermPrecedences.Morphism,
                // f:'1->'3->'4
                new InterpretTerm(
                    "->", "a",  // a:'1
                    (ic, a) =>
                        // '3->'4
                        new InterpretTerm(
                            $"Closure(-> {a})", "b",  // b:'3
                            (oc, b) =>
                                new FunctionTerm((IdentityTerm)a.VisitReduce(ic), b.VisitReduce(oc)))));
        }

        private Environment(ManagedDictionary<string, List<BoundTermInformation>> boundTerms) : base(boundTerms)
        { }

        public static Environment Create(bool pure = false) =>
            new Environment(pure ? new ManagedDictionary<string, List<BoundTermInformation>>() : defaultBoundTerms.Fork());
    }
}
