using BasicSyntaxTree.Types;
using BasicSyntaxTree.Types.Unresolved;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree
{
    internal sealed class InferContext
    {
        private readonly Dictionary<int, Type> types = new Dictionary<int, Type>();
        private int index;

        public UnspecifiedType CreateUnspecifiedType() =>
            new UnspecifiedType(this.index++);

        // =======================================================================

        private Type? GetInferredType(UnspecifiedType unspecifiedType) =>
            this.types.TryGetValue(unspecifiedType.Index, out var type) ? type : null;

        private void AddInferredType(UnspecifiedType unspecifiedType, Type type) =>
            this.types.Add(unspecifiedType.Index, type);

        private bool Occur(UnresolvedType type, UnspecifiedType unspecifiedType)
        {
            if (type is UnresolvedFunctionType ft)
            {
                return
                    Occur(ft.ParameterType, unspecifiedType) ||
                    Occur(ft.ExpressionType, unspecifiedType);
            }

            if (type is UnspecifiedType unspecifiedType2)
            {
                if (unspecifiedType2.Index == unspecifiedType.Index)
                {
                    return true;
                }

                if (this.GetInferredType(unspecifiedType2) is UnresolvedType it)
                {
                    return Occur(it, unspecifiedType);
                }
            }

            return false;
        }

        private void UnifyUnspecified(UnspecifiedType unspecifiedType, Type type)
        {
            //var isOccur = Occur(type, untypedType);
            //if (isOccur)
            //{
            //    throw new System.Exception();
            //}

            if (this.GetInferredType(unspecifiedType) is Type inferredType)
            {
                Unify(inferredType, type);
            }
            else
            {
                this.AddInferredType(unspecifiedType, type);
            }
        }

        public void Unify(Type type1, Type type2)
        {
            {
                // unify(UntypedFunctionType, ...)
                if (type1 is UnresolvedFunctionType ft11)
                {
                    if (type2 is UnresolvedFunctionType ft21)
                    {
                        Unify(ft11.ParameterType, ft21.ParameterType);
                        Unify(ft11.ExpressionType, ft21.ExpressionType);
                        return;
                    }
                    if (type2 is FunctionType ft22)
                    {
                        Unify(ft11.ParameterType, ft22.ParameterType);
                        Unify(ft11.ExpressionType, ft22.ExpressionType);
                        return;
                    }
                }
            }

            {
                // unify(FunctionType, ...)
                if (type1 is FunctionType ft11)
                {
                    if (type2 is UnresolvedFunctionType ft21)
                    {
                        Unify(ft11.ParameterType, ft21.ParameterType);
                        Unify(ft11.ExpressionType, ft21.ExpressionType);
                        return;
                    }
                    if (type2 is FunctionType ft22)
                    {
                        Unify(ft11.ParameterType, ft22.ParameterType);
                        Unify(ft11.ExpressionType, ft22.ExpressionType);
                        return;
                    }
                }
            }

            {
                // unify(UnspecifiedType)
                if (type1 is UnspecifiedType ut1)
                {
                    if (type2 is UnspecifiedType ut21)
                    {
                        if (ut1.Index == ut21.Index)
                        {
                            return;
                        }
                    }

                    UnifyUnspecified(ut1, type2);
                    return;
                }
                if (type2 is UnspecifiedType ut22)
                {
                    UnifyUnspecified(ut22, type1);
                    return;
                }
            }

            if (type1.Equals(type2))
            {
                return;
            }

//            throw new System.Exception();
        }

        public Type ResolveType(Type type)
        {
            if (type is UnresolvedFunctionType ft1)
            {
                return new FunctionType(
                    this.ResolveType(ft1.ParameterType),
                    this.ResolveType(ft1.ExpressionType));
            }
            if (type is FunctionType ft2)
            {
                return new FunctionType(
                    this.ResolveType(ft2.ParameterType),
                    this.ResolveType(ft2.ExpressionType));
            }
            if (type is UnresolvedClrType clrType)
            {
                return clrType.ToClrType();
            }
            if (type is UnspecifiedType unspecifiedType)
            {
                if (types.TryGetValue(unspecifiedType.Index, out var vt))
                {
                    return vt;
                }
            }
            return type;
        }

        // =======================================================================

        public override string ToString() =>
            $"[{string.Join(",", this.types.Select(entry => $"{{{entry.Key}:{entry.Value}}}"))}]";
    }
}
