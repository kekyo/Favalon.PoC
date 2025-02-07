﻿namespace BasicSyntaxTree.Types
{
    public sealed class RuntimeKindType : KindType, IRuntimeType
    {
        internal RuntimeKindType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool Equals(Type other) =>
            other is RuntimeKindType rhs ? this.Type.Equals(rhs.Type) :
            false;

        public override string ToString() =>
            this.Type.PrettyPrint();

        internal Type ToRuntimeType() =>
            Runtime(this.Type);
    }
}
