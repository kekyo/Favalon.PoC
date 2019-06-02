using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeInferences.Types.Internals
{
    internal sealed class UnionType : AvalonType
    {
        private readonly IEnumerable<AvalonType> types;

        public UnionType(IEnumerable<AvalonType> types) =>
            this.types = types;

        public override AvalonTypes Type =>
            AvalonTypes.Union;

        public override string Identity =>
            string.Format("{{{0}}}", string.Join(",", this.types));

        public IEnumerable<AvalonType> EnumerateTypes() =>
            this.types;

        internal override bool IsConvertibleFrom(AvalonType rhs)
        {
            return false;
        }
    }
}
