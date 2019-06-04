using System;
using System.Collections.Generic;

namespace BasicSyntaxTree
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
            environment.GetType(this.Name) ?? context.CreateVariable();

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

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context)
        {
            var scopedEnvironment = environment.MakeScope();
            var parameterType = context.CreateVariable();
            scopedEnvironment.RegisterVariable(this.Parameter, parameterType);
            return Type.Function(parameterType, this.Body.VisitInfering(scopedEnvironment, context));
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

        private static bool Occur(Type type, VariableType variableType, VariableContext context)
        {
            if (type is FunctionType ft)
            {
                return
                    Occur(ft.ParameterType, variableType, context) ||
                    Occur(ft.ExpressionType, variableType, context);
            }

            if (type is VariableType vt)
            {
                if (vt.Index == variableType.Index)
                {
                    return true;
                }
                
                if (context.GetInferType(vt) is Type it)
                {
                    return Occur(it, variableType, context);
                }
            }

            return false;
        }

        private static void Unify(VariableType variableType, Type type, VariableContext context)
        {
            var isOccur = Occur(type, variableType, context);
            if (isOccur)
            {
                throw new Exception();
            }

            if (context.GetInferType(variableType) is Type it)
            {
                Unify(it, type, context);
            }
            else
            {
                context.AddInferType(variableType, type);
            }
        }

        private static void Unify(Type type1, Type type2, VariableContext context)
        {
            if ((type1 is FunctionType ft1) && (type2 is FunctionType ft2))
            {
                Unify(ft1.ParameterType, ft2.ParameterType, context);
                Unify(ft1.ExpressionType, ft2.ExpressionType, context);
                return;
            }
            if (type1 is VariableType vt1)
            {
                if (type2 is VariableType vt21)
                {
                    if (vt1.Index == vt21.Index)
                    {
                        return;
                    }
                }

                Unify(vt1, type2, context);
                return;
            }
            if (type2 is VariableType vt22)
            {
                Unify(vt22, type1, context);
                return;
            }
            if (type1.Equals(type2))
            {
                return;
            }

            throw new Exception();
        }

        internal override Type VisitInfering(TypeEnvironment environment, VariableContext context)
        {
            var ft = this.Function.VisitInfering(environment, context);
            var at = this.Argument.VisitInfering(environment, context);
            var rt = context.CreateVariable();

            Unify(ft, Type.Function(at, rt), context);

            return rt;
        }

        public override string ToString() =>
            $"{this.Function} {this.Argument}";
    }
}
