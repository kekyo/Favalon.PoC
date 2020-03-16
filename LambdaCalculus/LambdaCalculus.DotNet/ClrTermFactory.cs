using Favalon.Terms;
using Favalon.Terms.Algebraic;
using Favalon.Terms.Logical;
using Favalon.Terms.Methods;
using Favalon.Terms.Types;
using System;
using System.Reflection;

namespace Favalon
{
    public sealed class ClrTermFactory : TermFactory
    {
        private ClrTermFactory()
        { }

        ///////////////////////////////////////////////////////////////////////////
        // CLR Constants

        public static new BooleanTerm True() =>
            ClrConstantTerm.True;
        public static new BooleanTerm False() =>
            ClrConstantTerm.False;

        public static Term Constant(Type type) =>
            ClrTypeTerm.From(type);
        public static Term Constant(MethodInfo method) =>
            ClrMethodTerm.From(method);
        public static BooleanTerm Constant(bool value) =>
            ClrConstantTerm.From(value);
        public static Term Constant(object value) =>
            ClrConstantTerm.From(value);

        ///////////////////////////////////////////////////////////////////////////
        // CLR Types

        public static Term ClrType<T>() =>
            ClrConstantTerm.From(typeof(T));

        public static WideningTerm WideningClrType(Term lhs, Term rhs) =>
            WideningTerm.Create(lhs, rhs, UnspecifiedTerm.Instance, ClrTypeCalculator.Instance);

        ///////////////////////////////////////////////////////////////////////////
        // CLR Methods

        public static Term ClrMethod<T>(string name, params Type[] argumentTypes) =>
            ClrMethodTerm.From(typeof(T).GetMethod(name, argumentTypes));
    }
}
