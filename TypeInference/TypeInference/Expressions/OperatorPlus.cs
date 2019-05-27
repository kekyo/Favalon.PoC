using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInference.Types;

namespace TypeInference.Expressions
{
    public sealed class OperatorPlus
    {
        private readonly Constant expression1;
        private readonly Constant expression2;

        public OperatorPlus(Constant expression1, Constant expression2)
        {
            this.expression1 = expression1;
            this.expression2 = expression2;
        }

        public IEnumerable<AvalonType> CalculatedTypes =>
            expression1.CalculatedTypes.Distinct().
            SelectMany(type1 => expression2.CalculatedTypes.Distinct().Select(type2 => (type1, type2))).
            Collect(entry => 
            {
                switch (entry.type1.CompareTo(entry.type2))
                {
                    case ComparedResult.Equal:
                        return Option.Some(entry.type1);
                    case ComparedResult.NarrowThan:
                        return Option.Some(entry.type1);
                    case ComparedResult.WideThan:
                        return Option.Some(entry.type2);
                    default:
                        return Option.None;
                }
            });
    }
}
