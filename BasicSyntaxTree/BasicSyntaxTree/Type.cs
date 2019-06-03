using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSyntaxTree
{
    public abstract class Type : IEquatable<Type>
    {
        protected Type() { }

        public abstract bool Equals(Type other);

        public static IntegerType Integer =>
            new IntegerType();

        public static FunctionType Function(Type parameterType, Type resultType) =>
            new FunctionType(parameterType, resultType);

        public static VariableType Variable(int index) =>
            new VariableType(index);
    }

    public sealed class IntegerType : Type
    {
        internal IntegerType() { }

        public override bool Equals(Type other) =>
            other is IntegerType;

        public override string ToString() =>
            "Integer";
    }

    public sealed class FunctionType : Type
    {
        public readonly Type ParameterType;
        public readonly Type ResultType;

        internal FunctionType(Type parameterType, Type resultType)
        {
            this.ParameterType = parameterType;
            this.ResultType = resultType;
        }

        public override bool Equals(Type other) =>
            other is FunctionType rhs ?
                (this.ParameterType.Equals(rhs.ParameterType) && this.ResultType.Equals(rhs.ResultType)) :
                false;

        public override string ToString() =>
            $"{this.ParameterType} -> {this.ResultType}";
    }

    public sealed class VariableType : Type
    {
        public readonly int Index;

        internal VariableType(int index) =>
            this.Index = index;

        public override bool Equals(Type other) =>
            other is VariableType rhs ?
                (this.Index == rhs.Index) :
                false;

        public override string ToString() =>
            $"'T{this.Index}";
    }
}
