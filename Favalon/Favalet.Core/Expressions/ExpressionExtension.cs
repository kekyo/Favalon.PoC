////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Contexts;
using Favalet.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Favalet.Expressions
{
    public interface IInferrableExpression : IExpression
    {
        IExpression Infer(IInferContext context);
        IExpression Fixup(IFixupContext context);
    }

    public interface IReducibleExpression : IExpression
    {
        IExpression Reduce(IReduceContext context);
    }

    public static class ExpressionExtension
    {
        public static IExpression InferIfRequired(
            this IInferrableExpression expression,
            IInferContext context) =>
            expression.Infer(context);
        public static IExpression InferIfRequired(
            this IExpression expression,
            IInferContext context) =>
            expression is IInferrableExpression i ?
                i.Infer(context) :
                expression;

        public static IExpression FixupIfRequired(
            this IInferrableExpression expression,
            IFixupContext context) =>
            expression.Fixup(context);
        public static IExpression FixupIfRequired(
            this IExpression expression,
            IFixupContext context) =>
            expression is IInferrableExpression i ?
                i.Fixup(context) :
                expression;

        public static IExpression ReduceIfRequired(
            this IReducibleExpression expression,
            IReduceContext context) =>
            expression.Reduce(context);
        public static IExpression ReduceIfRequired(
            this IExpression expression,
            IReduceContext context) =>
            expression is IReducibleExpression r ?
                r.Reduce(context) :
                expression;

        public static bool ShallowEquals(
            this IExpression expression,
            IExpression rhs) =>
            ShallowEqualityComparer.Equals(expression, rhs);
        public static bool ShallowSequenceEqual(
            this IEnumerable<IExpression> expressions,
            IEnumerable<IExpression> rhss) =>
            expressions.SequenceEqual(rhss, ShallowEqualityComparer.Instance);

        public static bool ExactEquals(
            this IExpression expression,
            IExpression rhs) =>
            ExactEqualityComparer.Equals(expression, rhs);
        public static bool ExactSequenceEqual(
            this IEnumerable<IExpression> expressions,
            IEnumerable<IExpression> rhss) =>
            expressions.SequenceEqual(rhss, ExactEqualityComparer.Instance);

        public static void WriteTo(
            this IExpression expression,
            XmlWriter writer)
        {
            var node = expression.Format(FormatXmlContext.Create());
            node.WriteTo(writer);
        }

        public static XNode FormatXml(
            this IExpression expression) =>
            expression.Format(FormatXmlContext.Create());

        public static string FormatXmlString(
            this IExpression expression,
            bool readable = true)
        {
            var node = expression.Format(FormatXmlContext.Create());
            if (node is XText text)
            {
                return text.Value;
            }
            else
            {
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(
                    sb,
                    new XmlWriterSettings
                    {
                        OmitXmlDeclaration = readable,
                        Indent = readable,
                        IndentChars = "  ",
                        NewLineChars = readable ? "\r\n" : string.Empty
                    }))
                {
                    node.WriteTo(writer);
                    writer.Flush();
                }
                return sb.ToString();
            }
        }
    }
}
