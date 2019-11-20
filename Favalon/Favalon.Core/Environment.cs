using Favalon.Internal;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed partial class Environment : Context
    {
        private static readonly Dictionary<string, List<BoundTermInformation>> defaultBoundTerms =
            typeof(object).GetAssembly().
            EnumerableAllPublicStaticMethods().
            Where(method => method.GetParameters().Length == 1).
            GroupBy(method => method.GetFullName()).
            ToDictionary(
                g => g.Key,
                g => g.Select(method => new BoundTermInformation(false, false, new MethodTerm(method))).ToList());

        static Environment()
        {
            // operator arrow (lambda constructor)
            // -> a b
            // --------------
            // f  a b
            // ((f:'1->'2 a:'1):'2 b:'3):'4
            // ((f:'1->'2 a:'1):'3->'4 b:'3):'4
            // ((f:'1->'3->'4 a:'1):'3->'4 b:'3):'4
            AddBoundTerm(
                defaultBoundTerms,
                "->", true, true,
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

        private Environment()
        { }

        private Environment(Dictionary<string, List<BoundTermInformation>> boundTerms) : base(boundTerms)
        { }

        public static Environment Create(bool pure = false) =>
            pure ? new Environment() : new Environment(defaultBoundTerms);
    }
}
