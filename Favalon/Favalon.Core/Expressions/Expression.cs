using System;

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        private protected Expression() { }

        public abstract string ReadableString { get; }

        public abstract Expression Infer(ExpressionEnvironment environment);

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.ReadableString}";

        ///////////////////////////////////////////////////////////

        public static IntegerExpression Integer(int value) =>
            new IntegerExpression(value);
 
        public static UnresolvedVariableExpression Variable(string name) =>
            new UnresolvedVariableExpression(name);

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument);

        public static BindExpression Bind(VariableExpression parameter, Expression expression, Expression body) =>
            new BindExpression(parameter, expression, body);

        public static TypeExpression Type(string name) =>
            new TypeExpression(name);

        public static KindExpression Kind() =>
            new KindExpression();
    }
}
