using Favalon.Terms;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    partial class Term
    {
        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;
        public static LambdaTerm UnspecifiedFunction() =>
            LambdaTerm.Unspecified;
        public static KindTerm Kind() =>
            KindTerm.Instance;
        public static LambdaTerm KindFunction() =>
            LambdaTerm.Kind;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static Term Type(Type type) =>
            TypeTerm.From(type);
        public static Term Type<T>() =>
            TypeTerm.From(typeof(T));

        public static MethodTerm Method(MethodInfo method) =>
            MethodTerm.From(new[] { method });
        public static MethodTerm Method(MethodInfo method0, params MethodInfo[] methods) =>
            MethodTerm.From(new[] { method0 }.Concat(methods));
        public static MethodTerm Method(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return ms.Length switch
            {
                0 => throw new ArgumentException(),
                _ => MethodTerm.From(ms)
            };
        }

        public static Term Constant(object value) =>
            ValueTerm.Create(value);

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);
        public static IdentityTerm Identity(string identity, Term higherOrder) =>
            new IdentityTerm(identity, higherOrder);

        public static ApplyTerm Apply(Term function, Term argument) =>
            ApplyTerm.Create(function, argument, UnspecifiedTerm.Instance);
        public static ApplyTerm Apply(Term function, Term argument, Term higherOrder) =>
            ApplyTerm.Create(function, argument, higherOrder);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            LambdaTerm.From(new IdentityTerm(parameter, UnspecifiedTerm.Instance), body);
        public static LambdaTerm Lambda(Term parameter, Term body) =>
            LambdaTerm.From(parameter, body);

        public static BindExpressionTerm Bind(string bound, Term body) =>
            new BindExpressionTerm(new IdentityTerm(bound, UnspecifiedTerm.Instance), body);
        public static BindExpressionTerm Bind(Term bound, Term body) =>
            new BindExpressionTerm(bound, body);

        public static BindTerm Bind(string bound, Term body, Term continuation) =>
            new BindTerm(new BindExpressionTerm(new IdentityTerm(bound, UnspecifiedTerm.Instance), body), continuation);
        public static BindTerm Bind(Term bound, Term body, Term continuation) =>
            new BindTerm(new BindExpressionTerm(bound, body), continuation);
    }
}
