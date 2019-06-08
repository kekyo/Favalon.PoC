using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression(TextRegion textRegion) : base(textRegion) { }

        public override bool IsResolved => false;

        internal abstract TypedExpression Visit(TypeEnvironment environment, InferContext context);

        public TypedExpression Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, Type>
        {
            var context = new InferContext();
            var typedExpression = this.Visit(new TypeEnvironment(typeEnvironment), context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================

        private static bool Occur(Type type, UntypedType untypedType, InferContext context)
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

        private static void Unify(UntypedType untypedType, Type type, InferContext context)
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

        internal static void Unify(Type type1, Type type2, InferContext context)
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
    }
}
