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
 
        public static MethodTerm Method(MethodInfo method) =>
            new MethodTerm(method);

        public static ValueTerm Constant(object constant) =>
            constant switch
            {
                Type type => new ClrTypeTerm(type),
                _ => new ConstantTerm(constant)
            };
    }
}
