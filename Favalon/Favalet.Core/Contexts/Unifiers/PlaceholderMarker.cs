using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Favalet.Internal;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerStepThrough]
    internal sealed class PlaceholderMarker
    {
        private readonly HashSet<int> indexes;
#if DEBUG
        private readonly List<int> list;
#endif
        private PlaceholderMarker(
#if DEBUG
            HashSet<int> indexes, List<int> list
#else
            HashSet<int> indexes
#endif
        )
        {
            this.indexes = indexes;
#if DEBUG
            this.list = list;
#endif
        }

        public bool Mark(int targetIndex)
        {
#if DEBUG
            list.Add(targetIndex);
#endif
            return indexes.Add(targetIndex);
        }

        public PlaceholderMarker Fork() =>
#if DEBUG
            new PlaceholderMarker(new HashSet<int>(this.indexes), new List<int>(this.list));
#else
            new PlaceholderMarker(new HashSet<int>(this.symbols));
#endif

#if DEBUG
        public override string ToString() =>
            StringUtilities.Join(" ==> ", this.list.Select(index => $"'{index}"));
#endif

        public static PlaceholderMarker Create() =>
#if DEBUG
            new PlaceholderMarker(new HashSet<int>(), new List<int>());
#else
             new PlaceholderMarker(new HashSet<int>());
#endif
    }
}
