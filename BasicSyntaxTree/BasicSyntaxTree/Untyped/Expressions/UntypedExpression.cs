using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Untyped.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression(TextRegion textRegion) : base(textRegion) { }

        public override bool IsResolved => false;

        internal abstract TypedExpression Visit(TypeEnvironment environment, InferContext context);

        public TypedExpression Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, UntypedType>
        {
            var context = new InferContext();
            var typedExpression = this.Visit(new TypeEnvironment(typeEnvironment), context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================

        private static bool Occur(UntypedType type, UnspecifiedType unspecifiedType, InferContext context)
        {
            if (type is UntypedFunctionType ft)
            {
                return
                    Occur(ft.ParameterType, unspecifiedType, context) ||
                    Occur(ft.ExpressionType, unspecifiedType, context);
            }

            if (type is UnspecifiedType untypedType2)
            {
                if (untypedType2.Index == unspecifiedType.Index)
                {
                    return true;
                }

                if (context.GetInferredType(untypedType2) is UntypedType it)
                {
                    return Occur(it, unspecifiedType, context);
                }
            }

            return false;
        }

        private static void Unify(UnspecifiedType unspecifiedType, UntypedType type, InferContext context)
        {
            //var isOccur = Occur(type, untypedType, context);
            //if (isOccur)
            //{
            //    throw new System.Exception();
            //}

            if (context.GetInferredType(unspecifiedType) is UntypedType inferredType)
            {
                Unify(inferredType, type, context);
            }
            else
            {
                context.AddInferredType(unspecifiedType, type);
            }
        }

        internal static void Unify(UntypedType type1, UntypedType type2, InferContext context)
        {
            if ((type1 is UntypedFunctionType functionType1) && (type2 is UntypedFunctionType functionType2))
            {
                Unify(functionType1.ParameterType, functionType2.ParameterType, context);
                Unify(functionType1.ExpressionType, functionType2.ExpressionType, context);
                return;
            }
            if (type1 is UnspecifiedType unspecifiedType1)
            {
                if (type2 is UnspecifiedType unspecifiedType21)
                {
                    if (unspecifiedType1.Index == unspecifiedType21.Index)
                    {
                        return;
                    }
                }

                Unify(unspecifiedType1, type2, context);
                return;
            }
            if (type2 is UnspecifiedType unspecifiedType22)
            {
                Unify(unspecifiedType22, type1, context);
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
