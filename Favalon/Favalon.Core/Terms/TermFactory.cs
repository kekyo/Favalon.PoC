using System;
using System.Reflection;

namespace Favalon.Terms
{
    partial class Term
    {
        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity);

        public static BoundIdentityTerm BoundIdentity(string identity) =>
            new BoundIdentityTerm(identity);

        public static FunctionTerm Function(BoundIdentityTerm parameter, Term body) =>
            new FunctionTerm(parameter, body);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);

        public static TypeTerm Type(Type type) =>
            new TypeTerm(type);
        public static TypeTerm Type<T>() =>
            new TypeTerm(typeof(T));

        public static Term TypeConstructor(Type gtd) =>
            TermUtilities.CreateTypeConstructorTerm(gtd);

        public static MethodTerm Method(MethodInfo method) =>
            new MethodTerm(method);

        public static Term Constant(object constant) =>
            constant switch
            {
                Type type => new TypeTerm(type),
                MethodBase method => new MethodTerm(method),
                _ => new ConstantTerm(constant)
            };
    }
}
