﻿using System;
using System.Reflection;

namespace Favalon.Terms
{
    public abstract class ValueTerm :
        Term, IEquatable<ValueTerm?>
    {
        internal ValueTerm()
        { }

        public abstract new object Constant { get; }

        public override Term HigherOrder =>
#if NET35 || NET40 || NET45
            new ClrTypeTerm(this.Constant.GetType());
#else
            new ClrTypeTerm(this.Constant.GetType().GetTypeInfo());
#endif

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitReduce(Context context) =>
            this;

        protected internal override Term VisitInfer(Context context) =>
            this;

        public override int GetHashCode() =>
            this.Constant?.GetHashCode() ?? 0;

        public bool Equals(ValueTerm? other) =>
            other?.Constant.Equals(this.Constant) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as ValueTerm);

        protected internal override string VisitTermString(bool includeTermName) =>
            this.Constant is string stringValue ?
                "\"" + stringValue + "\"" :
            this.Constant?.ToString() ?? "null";

        public void Deconstruct(out object constant) =>
            constant = this.Constant;
    }
}
