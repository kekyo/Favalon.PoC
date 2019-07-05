﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static Internals.StaticFactories;

    [TestFixture]
    public sealed class BindExpressionTest
    {
        [Test]
        public void Bind1()
        {
            var environment = Environment.Create();

            /*
            Bind 1:
            a = 123
            (a:? = 123:?):?
            1:-------------------
            (a:? = 123:?):'1
            (a:'1 = 123:?):'1
            (a:'1 = 123:Numeric):'1                      : Memoize('1 => Numeric)
            2:-------------------
            (a:Numeric = 123:Numeric):'1                 : Update('1 => Numeric)
            (a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
            3:-------------------
            Numeric
            */

            var expression = Bind(Bound("a"), Literal(123));
            Assert.AreEqual("(a:? = 123:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:Numeric = 123:Numeric):Numeric", inferred.StrictReadableString);
        }
    }
}
