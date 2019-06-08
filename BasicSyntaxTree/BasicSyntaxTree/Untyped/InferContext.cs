using BasicSyntaxTree.Untyped.Types;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree.Untyped
{
    internal sealed class InferContext
    {
        private readonly Dictionary<int, UntypedType> types = new Dictionary<int, UntypedType>();
        private int index;

        public UnspecifiedType CreateUnspecifiedType() =>
            Type.Unspecified(this.index++);

        public UntypedType? GetInferredType(UnspecifiedType unspecifiedType) =>
            this.types.TryGetValue(unspecifiedType.Index, out var type) ? type : null;

        public void AddInferredType(UnspecifiedType unspecifiedType, UntypedType type) =>
            this.types.Add(unspecifiedType.Index, type);

        public UntypedType ResolveType(UntypedType type)
        {
            if (type is UntypedFunctionType functionType)
            {
                return Type.Function(
                    this.ResolveType(functionType.ParameterType),
                    this.ResolveType(functionType.ExpressionType));
            }
            if (type is UnspecifiedType unspecifiedType)
            {
                if (types.TryGetValue(unspecifiedType.Index, out var vt))
                {
                    return this.ResolveType(vt);
                }
            }
            return type;
        }

        public override string ToString() =>
            $"[{string.Join(",", this.types.Select(entry => $"{{{entry.Key}:{entry.Value}}}"))}]";
    }
}
