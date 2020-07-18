using Favalet.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public static class CLRGenerator
    {
        public static TypeTerm Type<T>() =>
            TypeTerm.From(typeof(T));

        public static TypeTerm Type(Type runtimeType) =>
            TypeTerm.From(runtimeType);
    }
}
