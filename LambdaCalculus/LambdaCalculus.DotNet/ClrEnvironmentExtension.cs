using Favalon.Terms;
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
        private static Environment BindClrType(Environment environment, string identity, Type type)
        {
            var term = ClrTypeTerm.From(type);
            environment.BindTerm(identity, term);

            // TODO: constructor functions

            return environment;
        }

        public static Environment BindClrType(this Environment environment, Type type) =>
            BindClrType(environment, type.GetFullName(false), type);

        public static Environment BindClrType(this Environment environment, IEnumerable<Type> types) =>
            types.Aggregate(environment, BindClrType);

        public static Environment BindClrType(this Environment environment, params Type[] types) =>
            BindClrType(environment, (IEnumerable<Type>)types);

        public static Environment BindClrPublicTypes(this Environment environment, Assembly assembly) =>
            BindClrType(environment, assembly.GetTypes().Where(type => type.IsPublic()));

        public static Environment BindClrAliasTypes(this Environment environment) =>
            new[]
            {
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
                ("string", typeof(string)),
            }.Aggregate(environment, (env, entry) => BindClrType(env, entry.Item1, entry.Item2));

        public static Environment BindClrBooleanConstant(this Environment environment) =>
            environment.
                BindTerm("true", ClrConstantTerm.From(true)).
                BindTerm("false", ClrConstantTerm.From(false));

        public static Environment BindClrTypeOperators(this Environment environment) =>
            environment.
                BindTerm("+", ClrTypeSumOperatorTerm.Instance).
                BindTerm("*", ClrTypeProductOperatorTerm.Instance);
    }
}
