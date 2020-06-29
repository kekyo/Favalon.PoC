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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Favalet.Expressions;
using Favalet.Expressions.Specialized;

namespace Favalet.Contexts
{
    [Flags]
    public enum FormatOptions
    {
        Standard = 0x00,
        SuppressHigherOrder = 0x01,
        ForceText = 0x02,
    }

    public interface IFormatContext<T>
    {
        T Format(
            IExpression expression,
            FormatOptions options,
            params IExpression[] children);
        T Format(
            IExpression expression,
            FormatOptions options,
            string formattedString);

        string GetPlaceholderIndexString(int index);
    }

    public abstract class FormatContext
    {
        private static readonly char[] alphabets =
            Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (char)i).ToArray();

        private readonly Dictionary<int, int>? relativeIndexes;

        protected FormatContext() =>
            this.relativeIndexes = new Dictionary<int, int>();

        protected static string FormatExpressionName(IExpression expression)
        {
            var name = expression.GetType().Name;
            if (name.EndsWith("Expression"))
            {
                name = name.Substring(0, name.Length - "Expression".Length);
            }
            else if (name.EndsWith("Term"))
            {
                name = name.Substring(0, name.Length - "Term".Length);
            }

            return name;
        }

        public string GetPlaceholderIndexString(int index)
        {
            if (this.relativeIndexes is Dictionary<int, int> relativeIndexes)
            {
                if (!relativeIndexes.TryGetValue(index, out var relativeIndex))
                {
                    relativeIndex = relativeIndexes.Count;
                    relativeIndexes.Add(index, relativeIndex);
                }

                var sb = new StringBuilder();
                while (true)
                {
                    var i = relativeIndex % alphabets.Length;
                    var ch = alphabets[i];
                    sb.Insert(0, ch);

                    if (relativeIndex == 0)
                    {
                        return sb.ToString();
                    }

                    relativeIndex = relativeIndex / alphabets.Length;
                }
            }
            else
            {
                return index.ToString();
            }
        }
    }

    public sealed class FormatXmlContext :
        FormatContext, IFormatContext<XNode>
    {
        private FormatXmlContext()
        { }


        public XNode Format(
            IExpression expression,
            FormatOptions options,
            params IExpression[] children)
        {
            var name = FormatExpressionName(expression);

            var higherOrder = expression.HigherOrder;
            if (((options & FormatOptions.SuppressHigherOrder) != FormatOptions.SuppressHigherOrder) &&
                !(higherOrder is TerminationTerm))
            {
                var formattedHigherOrder = higherOrder.Format(this);
                var formattedHigherOrderNode =
                    formattedHigherOrder is XText textedHigherOrder ?
                        (XObject)new XAttribute("HigherOrder", textedHigherOrder.Value) :
                        new XElement(name + ".HigherOrder", formattedHigherOrder);

                return new XElement(name,
                    children.Select(child => child.Format(this)).
                    Cast<XObject>().
                    Append(formattedHigherOrderNode));
            }
            else
            {
                return new XElement(name,
                    children.Select(child => child.Format(this)));
            }
        }

        public XNode Format(
            IExpression expression,
            FormatOptions options,
            string formattedString)
        {
            var name = FormatExpressionName(expression);

            var higherOrder = expression.HigherOrder;
            if (((options & FormatOptions.SuppressHigherOrder) != FormatOptions.SuppressHigherOrder) &&
                !(higherOrder is TerminationTerm))
            {
                var formattedHigherOrder = higherOrder.Format(this);

                if (((options & FormatOptions.ForceText) == FormatOptions.ForceText) &&
                    formattedHigherOrder is XText textedHigherOrder)
                {
                    return new XText($"{formattedString}:{textedHigherOrder.Value}");
                }
                else
                {
                    var formattedHigherOrderNode =
                        formattedHigherOrder is XText textedHigherOrder2 ?
                            (XObject)new XAttribute("HigherOrder", textedHigherOrder2.Value) :
                            new XElement(name + ".HigherOrder", formattedHigherOrder);

                    return new XElement(name,
                        formattedString,
                        formattedHigherOrderNode);
                }
            }
            else
            {
                if ((options & FormatOptions.ForceText) == FormatOptions.ForceText)
                {
                    return new XText(formattedString);
                }
                else
                {
                    return new XElement(name, formattedString);
                }
            }
        }

        public static FormatXmlContext Create() =>
            new FormatXmlContext();
    }
}
