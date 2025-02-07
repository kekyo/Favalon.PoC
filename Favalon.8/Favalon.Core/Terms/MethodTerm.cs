﻿using Favalon.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class MethodTerm :
        CallableTerm, IEquatable<MethodTerm>
    {
        public new readonly MethodBase Method;

        internal MethodTerm(MethodBase method) =>
            this.Method = method;

        public override Term HigherOrder =>
            new TypeTerm(this.Method is MethodInfo mi ? mi.ReturnType : this.Method.DeclaringType);

        public override BoundIdentityTerm Parameter =>
            new BoundIdentityTerm(this.Method.GetParameters().Single().Name  /* TODO: , this.Method.GetParameters().Single().ParameterType */);

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override Term VisitCall(Context context, Term argument) =>
            new ConstantTerm(
                this.Method.Invoke(
                    null,
                    new object[] { ((ConstantTerm)context.Reduce(argument)).Constant }));

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^
            this.Method.GetHashCode();

        public bool Equals(MethodTerm? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Method.Equals(this.Method) ?? false);

        public override bool Equals(object obj) =>
            this.Equals(obj as MethodTerm);

        protected internal override string VisitTermString(bool includeTermName) =>
            this.Method is MethodInfo ?
                $"{this.Method.GetFullName()}({this.Parameter.ToString(includeTermName)})" :
                $"{this.Method.DeclaringType.GetFullName()}({this.Parameter.ToString(includeTermName)})";

        public void Deconstruct(out MethodBase method) =>
            method = this.Method;
    }
}
