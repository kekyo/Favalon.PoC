﻿using System.Reflection;

namespace Favalon.Terms
{
    public static class Factories
    {
        private static readonly Variable arrow = new Variable("->");

        public static Variable Variable(string name) =>
            new Variable(name);

        public static Variable Variable(string name, Term higherOrder) =>
            new Variable(name, higherOrder);

        public static Number Number(string number) =>
            new Number(number);

        public static Number<T> Number<T>(T number)
            where T : struct =>
            new Number<T>(number);

        public static String String(string stringValue) =>
            new String(stringValue);

        public static Apply Apply(Term function, Term argument) =>
            new Apply(function, argument);

        public static TypeSymbol TypeSymbol(TypeInfo type) =>
            new TypeSymbol(type);

        public static MethodSymbol MethodSymbol(MethodInfo method) =>
            new MethodSymbol(method);

        public static ExecutableSymbol ExecutableSymbol(string path) =>
            new ExecutableSymbol(path);

        public static Term Function(Term parameter, Term body) =>
            (!parameter.Equals(Unspecified.Instance) && !body.Equals(Unspecified.Instance)) ?
                (Term)Apply(Apply(parameter, arrow), body) :
                Unspecified.Instance;
    }
}
