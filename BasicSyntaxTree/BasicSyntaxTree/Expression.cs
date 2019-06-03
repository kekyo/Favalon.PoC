using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace BasicSyntaxTree
{
    public abstract class Expression
    {
        protected Expression() { }

        private protected VariableType CreateVariableType(ImmutableDictionary<string, Type> typeEnvironment) =>
            Type.Variable(typeEnvironment.Count);

        internal abstract Type VisitInfering(ImmutableDictionary<string, Type> typeEnvironment);

        public Type Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, Type> =>
            this.VisitInfering(typeEnvironment.ToImmutableDictionary());

        // =======================================================================

        public static NaturalExpression Natural(int value) =>
            new NaturalExpression(value);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name);

        public static FunctionExpression Function(string parameter, Expression body) =>
            new FunctionExpression(parameter, body);

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument);
    }

    public sealed class NaturalExpression : Expression
    {
        public readonly int Value;

        internal NaturalExpression(int value) =>
            this.Value = value;

        internal override Type VisitInfering(ImmutableDictionary<string, Type> typeEnvironment) =>
            Type.Integer;

        public override string ToString() =>
            $"{this.Value}:Integer";
    }

    public sealed class VariableExpression : Expression
    {
        public readonly string Name;

        internal VariableExpression(string name) =>
            this.Name = name;

        internal override Type VisitInfering(ImmutableDictionary<string, Type> typeEnvironment) =>
            typeEnvironment.TryGetValue(this.Name, out var type) ? type : throw new KeyNotFoundException($"Variable: {this.Name}");

        public override string ToString() =>
            this.Name;
    }

    public sealed class FunctionExpression : Expression
    {
        public readonly string Parameter;
        public readonly Expression Body;

        internal FunctionExpression(string parameter, Expression body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override Type VisitInfering(ImmutableDictionary<string, Type> typeEnvironment)
        {
            var parameterType = CreateVariableType(typeEnvironment);
            return Type.Function(parameterType, this.Body.VisitInfering(typeEnvironment.SetItem(this.Parameter, parameterType)));
        }

        public override string ToString() =>
            $"fun {this.Parameter} = {this.Body}";
    }

    public sealed class ApplyExpression : Expression
    {
        public new readonly Expression Function;
        public readonly Expression Argument;

        internal ApplyExpression(Expression function, Expression argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        private static void Unify(ImmutableDictionary<string, Type> typeEnvironment, Type functionType, Type t2)
        {

        }

        internal override Type VisitInfering(ImmutableDictionary<string, Type> typeEnvironment)
        {
            var functionType = this.Function.VisitInfering(typeEnvironment);
            var argumentType = this.Argument.VisitInfering(typeEnvironment);
            var returnType = CreateVariableType(typeEnvironment);

            Unify(typeEnvironment, functionType, Type.Function(argumentType, returnType));

            return returnType;
        }

        public override string ToString() =>
            $"{this.Function} {this.Argument}";
    }
}
