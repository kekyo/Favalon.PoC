using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Expressions
{
    public abstract class Expression
    {
        protected Expression() { }

        internal abstract Type VisitInfering(TypeEnvironment environment, VariableContext context);

        public Type Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, Type>
        {
            var context = new VariableContext();
            return context.Resolve(this.VisitInfering(new TypeEnvironment(typeEnvironment), context));
        }

        // =======================================================================

        public static NaturalExpression Natural(int value) =>
            new NaturalExpression(value);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name);

        public static LambdaExpression Lambda(string parameter, Expression body) =>
            new LambdaExpression(parameter, body);

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument);
    }

    public sealed class NaturalExpression : Expression
    {
        public readonly int Value;

        internal NaturalExpression(int value) =>
            this.Value = value;

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context) =>
            Type.Integer;

        public override string ToString() =>
            $"{this.Value}:Integer";
    }

    public sealed class VariableExpression : Expression
    {
        public readonly string Name;

        internal VariableExpression(string name) =>
            this.Name = name;

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context) =>
            environment.GetType(this.Name) ?? context.CreateUntypedType();

        public override string ToString() =>
            this.Name;
    }

    public sealed class LambdaExpression : Expression
    {
        public readonly string Parameter;
        public readonly Expression Body;

        internal LambdaExpression(string parameter, Expression body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context)
        {
            var scopedEnvironment = environment.MakeScope();
            var parameterType = context.CreateUntypedType();
            scopedEnvironment.RegisterVariable(this.Parameter, parameterType);
            return Type.Function(parameterType, this.Body.VisitInfering(scopedEnvironment, context));
        }

        public override string ToString() =>
            $"fun {this.Parameter} = {this.Body}";
    }

    public sealed class ApplyExpression : Expression
    {
        public new readonly Expression Lambda;
        public readonly Expression Argument;

        internal ApplyExpression(Expression lambda, Expression argument)
        {
            this.Lambda = lambda;
            this.Argument = argument;
        }

        private static bool Occur(Type type, UntypedType untypedType, VariableContext context)
        {
            if (type is FunctionType ft)
            {
                return
                    Occur(ft.ParameterType, untypedType, context) ||
                    Occur(ft.ExpressionType, untypedType, context);
            }

            if (type is UntypedType untypedType2)
            {
                if (untypedType2.Index == untypedType.Index)
                {
                    return true;
                }
                
                if (context.GetInferredType(untypedType2) is Type it)
                {
                    return Occur(it, untypedType, context);
                }
            }

            return false;
        }

        private static void Unify(UntypedType untypedType, Type type, VariableContext context)
        {
            //var isOccur = Occur(type, untypedType, context);
            //if (isOccur)
            //{
            //    throw new System.Exception();
            //}

            if (context.GetInferredType(untypedType) is Type inferredType)
            {
                Unify(inferredType, type, context);
            }
            else
            {
                context.AddInferredType(untypedType, type);
            }
        }

        private static void Unify(Type type1, Type type2, VariableContext context)
        {
            if ((type1 is FunctionType functionType1) && (type2 is FunctionType functionType2))
            {
                Unify(functionType1.ParameterType, functionType2.ParameterType, context);
                Unify(functionType1.ExpressionType, functionType2.ExpressionType, context);
                return;
            }
            if (type1 is UntypedType untypedType1)
            {
                if (type2 is UntypedType untypedType21)
                {
                    if (untypedType1.Index == untypedType21.Index)
                    {
                        return;
                    }
                }

                Unify(untypedType1, type2, context);
                return;
            }
            if (type2 is UntypedType untypedType22)
            {
                Unify(untypedType22, type1, context);
                return;
            }
            if (type1.Equals(type2))
            {
                return;
            }

            throw new System.Exception();
        }

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context)
        {
            var functionType = this.Lambda.VisitInfering(environment, context);
            var argumentType = this.Argument.VisitInfering(environment, context);
            var returnType = context.CreateUntypedType();

            Unify(functionType, Type.Function(argumentType, returnType), context);

            return returnType;
        }

        public override string ToString() =>
            $"{this.Lambda} {this.Argument}";
    }
}
