﻿using System.Reflection;

namespace Favalon
{
    public sealed class ClrEnvironment : Environment
    {
        private ClrEnvironment(int iterations) :
            base(iterations)
        {
            this.BoundPublicTypes(typeof(object).GetAssembly());
            this.BindCSharpTypes();
            this.BindClrTypeOperators();
            this.BindBooleanConstant();
        }

        public static new ClrEnvironment Create(int iterations = DefaultIterations) =>
            new ClrEnvironment(iterations);
    }
}
