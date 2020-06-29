using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions
{
    public interface IExpression : IEquatable<IExpression?>
    {
        IExpression Reduce(IReduceContext context);
    }
}
