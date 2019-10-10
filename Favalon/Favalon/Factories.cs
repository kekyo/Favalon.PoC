﻿namespace Favalon
{
    public static class Factories
    {
        public static Variable Variable(string name) =>
            new Variable(name);

        public static Number Number(int number) =>
            new Number(number);

        public static Apply Apply(Term function, Term argument) =>
            new Apply(function, argument);

        public static Function Function(Term parameter, Term body) =>
            new Function(parameter, body);
    }
}
