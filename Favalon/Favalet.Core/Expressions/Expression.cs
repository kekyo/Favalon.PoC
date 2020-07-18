﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions
{
    public enum PrettyStringTypes
    {
        Simple,
        Strict
    }

    public interface IExpression : IEquatable<IExpression?>
    {
        IExpression Reduce(IReduceContext context);

        string GetPrettyString(PrettyStringTypes type);
    }

    [DebuggerDisplay("{DebugPrint}")]
    public abstract class Expression
    {
        protected Expression()
        { }

        public abstract string GetPrettyString(PrettyStringTypes type);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebugPrint =>
            this.GetPrettyString(PrettyStringTypes.Simple);

        public string StrictExpression =>
            this.GetPrettyString(PrettyStringTypes.Strict);

        public override sealed string ToString() =>
            this.GetPrettyString(PrettyStringTypes.Strict);
    }
}
