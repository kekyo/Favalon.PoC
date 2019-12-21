﻿using Favalon.Contexts;
using Favalon.Terms;
using Favalon.Terms.Operators;
using Favalon.Terms.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    class OverloadTest
    {
        [Test]
        public void NonOverloadedMethod()
        {
            var term =
                Term.Apply(
                    Term.Method(typeof(int).GetMethod("Parse", new[] { typeof(string) })),
                    Term.Constant("123"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Constant(123), actual);
        }

        [TestCase(123, "123")]
        [TestCase(123, 123)]
        [TestCase(123, 123.456)]
        public void OverloadedMethods(int expected, object value)
        {
            var term =
                Term.Apply(
                    Term.Method(typeof(Convert).GetMethods().
                        Where(method => (method.Name == "ToInt32") && (method.GetParameters().Length == 1)).
                        ToArray()),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Constant(expected), actual);
        }

        [TestCase(123, typeof(int))]
        [TestCase(123.0, typeof(double))]
        public void OverloadedMethodsWithDifferentReturnType(object expected, Type required)
        {
            var m1 = typeof(Convert).GetMethod("ToInt32", new[] { typeof(string) });
            var m2 = typeof(Convert).GetMethod("ToInt64", new[] { typeof(string) });
            var m3 = typeof(Convert).GetMethod("ToDouble", new[] { typeof(string) });

            var term =
                Term.Apply(
                    Term.Method(m1, m2, m3),
                    Term.Constant("123"),
                    Term.Type(required));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Constant(expected), actual);
        }
    }
}
