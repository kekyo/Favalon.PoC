using Favalon.Terms;
using Favalon.Terms.Methods;
using Favalon.Terms.Operators;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    public static class ClrEnvironmentExtension
    {
        public static Environment BindMutableClrConstructor(this Environment environment, string identity, ConstructorInfo constructor)
        {
            //var term = ClrMethodTerm.From(constructor);
            //environment.BindMutable(identity, term);

            return environment;
        }

        public static Environment BindMutableClrMethod(this Environment environment, string identity, MethodInfo method)
        {
            var term = ClrMethodTerm.From(method);
            environment.BindMutable(identity, term);

            return environment;
        }

        private static Environment BindMutableClrType(Environment environment, string identity, Type type)
        {
            var term = ClrTypeTerm.From(type);
            environment.BindMutable(identity, term);

            if (!type.IsGenericType() || !type.IsGenericTypeDefinition())
            {
                foreach (var constructor in type.GetDeclaredConstructors())
                {
                    BindMutableClrConstructor(environment, identity, constructor);
                }

                // TODO: instance method
                foreach (var method in type.GetDeclaredMethods().Where(m => m.IsStatic))
                {
                    BindMutableClrMethod(environment, method.GetName(NameOptions.Symbols), method);
                }
            }

            return environment;
        }

        public static Environment BindMutableClrType(this Environment environment, Type type) =>
            BindMutableClrType(environment, type.GetFullName(false), type);

        public static Environment BindMutableClrType(this Environment environment, IEnumerable<Type> types) =>
            types.Aggregate(environment, BindMutableClrType);

        public static Environment BindMutableClrType(this Environment environment, params Type[] types) =>
            BindMutableClrType(environment, (IEnumerable<Type>)types);

        public static Environment BindMutableClrPublicTypes(this Environment environment, Assembly assembly) =>
            BindMutableClrType(environment, assembly.GetTypes().Where(type => type.IsPublic()));

        public static Environment BindMutableClrAliasTypes(this Environment environment) =>
            new[]
            {
                ("void", typeof(void)),
                ("object", typeof(object)),
                ("bool", typeof(bool)),
                ("byte", typeof(byte)),
                ("sbyte", typeof(sbyte)),
                ("short", typeof(short)),
                ("ushort", typeof(ushort)),
                ("int", typeof(int)),
                ("uint", typeof(uint)),
                ("long", typeof(long)),
                ("ulong", typeof(ulong)),
                ("float", typeof(float)),
                ("double", typeof(double)),
                ("decimal", typeof(decimal)),
                ("char", typeof(char)),
                ("guid", typeof(Guid)),
                ("string", typeof(string)),
                ("intptr", typeof(IntPtr)),
                ("uintptr", typeof(UIntPtr)),
                ("unit", typeof(Unit)),
            }.Aggregate(environment, (env, entry) => BindMutableClrType(env, entry.Item1, entry.Item2));

        public static Environment BindMutableClrConstants(this Environment environment) =>
            environment.
                BindMutable("()", ClrConstantTerm.From(Unit.Value)).
                BindMutable("true", ClrConstantTerm.From(true)).
                BindMutable("false", ClrConstantTerm.From(false));

        public static Environment BindMutableClrTypeOperators(this Environment environment) =>
            environment.
                BindMutable("+", ClrTypeSumOperatorTerm.Instance).
                BindMutable("*", ClrTypeProductOperatorTerm.Instance);
    }
}
