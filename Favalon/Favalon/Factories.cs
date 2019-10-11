namespace Favalon
{
    public static class Factories
    {
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

        public static Function Function(Term parameter, Term body) =>
            new Function(parameter, body);
    }
}
