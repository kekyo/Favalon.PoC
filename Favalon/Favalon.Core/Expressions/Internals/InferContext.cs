using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly Queue<Action> resolvers = new Queue<Action>();

        [DebuggerNonUserCode]
        internal InferContext()
        { }

        public int Rank { get; private set; }

        [DebuggerNonUserCode]
        internal void RaiseRank() =>
            this.Rank++;
        [DebuggerNonUserCode]
        internal void DropRank() =>
            this.Rank--;

        [DebuggerNonUserCode]
        public void Register(Action resolver) =>
            resolvers.Enqueue(resolver);

        internal void Resolve()
        {
            while (resolvers.Count >= 1)
            {
                resolvers.Dequeue()();
            }
        }
    }
}
