using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    public static class ClrEnvironmentExtension
    {
        private static void BindType(Environment environment, string identity, Type type)
        {
            var term = ClrTypeTerm.From(type);
            environment.SetBindTerm(identity, term);

            // TODO: constructor functions
        }

        public static void BindType(this Environment environment, Type type) =>
            BindType(environment, type.GetFullName(false), type);

        public static void BindType(this Environment environment, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                BindType(environment, type);
            }
        }

        public static void BindType(this Environment environment, params Type[] types) =>
            BindType(environment, types);

        public static void BoundPublicTypes(this Environment environment, Assembly assembly) =>
            BindType(environment, assembly.GetTypes().Where(type => type.IsPublic()));

        public static void BindCSharpTypes(this Environment environment)
        {
            foreach (var (identity, type) in new[]
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
            })
            {
                BindType(environment, identity, type);
            }
        }
    }
}
