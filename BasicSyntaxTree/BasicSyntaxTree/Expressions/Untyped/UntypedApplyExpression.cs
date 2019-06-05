using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public sealed class UntypedApplyExpression : UntypedExpression
    {
        public new readonly UntypedExpression Lambda;
        public readonly UntypedExpression Argument;

        internal UntypedApplyExpression(UntypedExpression lambda, UntypedExpression argument)
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

        internal override TypedExpression Visit(TypeEnvironment environment, VariableContext context)
        {
            var functionExpression = this.Lambda.Visit(environment, context);
            var argumentExpression = this.Argument.Visit(environment, context);
            var returnType = context.CreateUntypedType();

            Unify(functionExpression.Type, Type.Function(argumentExpression.Type, returnType), context);

            return new ApplyExpression(functionExpression, argumentExpression, returnType);
        }

        public override string ToString() =>
            $"{this.Lambda} {this.Argument}";
    }
}
