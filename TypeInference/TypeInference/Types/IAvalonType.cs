using System;

namespace TypeInferences.Types
{
    public enum AvalonTypes
    {
        Unspecified,
        ClrType,
        Union
    }

    public interface IAvalonType :
        IEquatable<IAvalonType>, IComparable<IAvalonType>
    {
        AvalonTypes Type { get; }

        string Identity { get; }

        AvalonType Normalized { get; }

        bool IsConvertibleFrom(IAvalonType rhs);
    }
}
