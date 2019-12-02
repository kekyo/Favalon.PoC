using System;
using System.Linq;
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

        public static MethodTerm ValueConstructor(Type type) =>
            new MethodTerm(type.GetConstructors().Single(constructor => constructor.GetParameters().Length == 1));
        public static MethodTerm ValueConstructor<T>() =>
            ValueConstructor(typeof(T));

        public static Term TypeConstructor(Type gtd) =>
            TermUtilities.CreateTypeConstructorTerm(gtd);

        public static MethodTerm Method(MethodInfo method) =>
            new MethodTerm(method);

        public static Term Constant(object constant) =>
            constant switch
            {
                Type gtd when gtd.IsGenericTypeDefinition() => TypeConstructor(gtd),
                Type type => ValueConstructor(type),
                MethodBase method => new MethodTerm(method),
                true => BooleanTerm.True,
                false => BooleanTerm.False,
                _ => new ConstantTerm(constant)
            };
    }
}
