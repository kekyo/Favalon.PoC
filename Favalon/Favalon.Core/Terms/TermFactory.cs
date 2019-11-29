using System;
using System.Reflection;

namespace Favalon.Terms
{
    partial class Term
    {
        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity);

        public static FunctionTerm Function(IdentityTerm parameter, Term body) =>
            new FunctionTerm(parameter, body);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);
 
        public static MethodTerm Method(MethodInfo method) =>
            new MethodTerm(method);

        public static ValueTerm Constant(object constant) =>
            constant switch
            {
#if NET35 || NET40 || NET45
                Type type => new ClrTypeTerm(type),
#else
                Type type => new ClrTypeTerm(type.GetTypeInfo()),
                TypeInfo type => new ClrTypeTerm(type),
#endif
                _ => new ConstantTerm(constant)
            };
    }
}
