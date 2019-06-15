using BasicSyntaxTree.Types;
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

        private bool Occur(Type type, UnspecifiedType unspecifiedType)
        {
            if (type is FunctionType ft)
            {
                return
                    Occur(ft.ParameterType, unspecifiedType) ||
                    Occur(ft.ResultType, unspecifiedType);
            }

            if (type is UnspecifiedType unspecifiedType2)
            {
                if (unspecifiedType2.Index == unspecifiedType.Index)
                {
                    return true;
                }

                if (this.GetInferredType(unspecifiedType2) is Type it)
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
            if (type1.Equals(type2))
            {
                return;
            }

            {
                // unify(FunctionType, ...)
                if (type1 is FunctionType ft11)
                {
                    // unify(FunctionType, FunctionType)
                    if (type2 is FunctionType ft)
                    {
                        Unify(ft11.ParameterType, ft.ParameterType);
                        Unify(ft11.ResultType, ft.ResultType);
                        return;
                    }
                    // unify(FunctionType, KindFunctionType)
                    else if (type2 is KindFunctionType kft1)
                    {
                        Unify(ft11.ParameterType, kft1.ParameterType);
                        Unify(ft11.ResultType, kft1.ResultType);
                        return;
                    }
                }
                // unify(KindFunctionType, ...)
                else if (type1 is KindFunctionType kft2)
                {
                    // unify(KindFunctionType, FunctionType)
                    if (type2 is FunctionType ft12)
                    {
                        Unify(kft2.ParameterType, ft12.ParameterType);
                        Unify(kft2.ResultType, ft12.ResultType);
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

//            throw new System.Exception();
        }

        public Type ResolveType(Type type)
        {
            if (type is FunctionType ft)
            {
                return new FunctionType(
                    this.ResolveType(ft.ParameterType),
                    this.ResolveType(ft.ResultType));
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
