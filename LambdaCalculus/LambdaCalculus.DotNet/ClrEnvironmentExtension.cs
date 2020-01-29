﻿using Favalon.Terms;
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
            environment.BindMutable(identity, term);

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
                ("unit", typeof(Unit)),
            }.Aggregate(environment, (env, entry) => BindClrType(env, entry.Item1, entry.Item2));

        public static Environment BindClrConstants(this Environment environment) =>
            environment.
                BindMutable("()", ClrConstantTerm.From(Unit.Value)).
                BindMutable("true", ClrConstantTerm.From(true)).
                BindMutable("false", ClrConstantTerm.From(false));

        public static Environment BindClrTypeOperators(this Environment environment) =>
            environment.
                BindMutable("+", ClrTypeSumOperatorTerm.Instance).
                BindMutable("*", ClrTypeProductOperatorTerm.Instance);
    }
}
