﻿using Favalon.Terms;
using Favalon.Terms.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

namespace Favalon
{
    [TestFixture]
    class OverloadTest
    {
        [Test]
        public void NonOverloadedMethod()
        {
            var term =
                Apply(
                    ClrMethod<int>("Parse", typeof(string)),
                    Constant("123"));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Constant(123), actual);
        }

        [TestCase(123, "123")]
        [TestCase(123, 123)]
        [TestCase(123, 123.456)]
        public void OverloadedMethods(int expected, object value)
        {
            var term =
                Apply(
                    SumType(
                        typeof(Convert).GetMethods().
                        Where(method => (method.Name == "ToInt32") && (method.GetParameters().Length == 1)).
                        Select(Constant))!,
                    Constant(value));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Constant(expected), actual);
        }

        //[TestCase(123, typeof(int))]
        //[TestCase(123.0, typeof(double))]
        //public void OverloadedMethodsWithDifferentReturnType(object expected, Type required)
        //{
        //    var m1 = typeof(Convert).GetMethod("ToInt32", new[] { typeof(object) });
        //    var m2 = typeof(Convert).GetMethod("ToInt64", new[] { typeof(object) });
        //    var m3 = typeof(Convert).GetMethod("ToDouble", new[] { typeof(object) });
        //    var m4 = typeof(Convert).GetMethod("ToInt32", new[] { typeof(string) });
        //    var m5 = typeof(Convert).GetMethod("ToInt64", new[] { typeof(string) });
        //    var m6 = typeof(Convert).GetMethod("ToDouble", new[] { typeof(string) });

        //    var term =
        //        Term.Apply(
        //            Term.Method(m1, m2, m3, m4, m5, m6),
        //            Term.Constant("123"),
        //            Term.Type(required));

        //    var environment = Environment.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(Term.Constant(expected), actual);
        //}

        //[TestCase(typeof(int), typeof(double))]
        //public void OverloadedMethodsFromAnnotatedReturnTypes(params Type[] requiredTypes)
        //{
        //    var ms = typeof(Convert).GetMethods(
        //        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).
        //        Where(m => m.GetParameters().Length == 1).
        //        ToArray();

        //    var expected =
        //        Term.Sum(
        //            requiredTypes.Select(t => Term.Type(t)));

        //    var term =
        //        Term.Apply(
        //            Term.Method(ms),
        //            Term.Constant("123"),
        //            expected);

        //    var environment = Environment.Create();
        //    var actual = environment.Infer(term);

        //    Assert.AreEqual(expected, actual.HigherOrder);
        //}
    }
}
