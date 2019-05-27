using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInference.Types
{
    public abstract class AvalonType : IEquatable<AvalonType>
    {
        protected AvalonType()
        {
        }

        public abstract bool Equals(AvalonType other);
    }
}
