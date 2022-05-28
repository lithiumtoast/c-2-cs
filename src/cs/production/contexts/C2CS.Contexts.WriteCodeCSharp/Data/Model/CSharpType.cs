// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace C2CS.Contexts.WriteCodeCSharp.Data.Model;

public sealed record CSharpType : IEquatable<CSharpType>
{
    public string Name { get; init; } = string.Empty;

    public string? OriginalName { get; init; }

    public int SizeOf { get; init; }

    public int? AlignOf { get; init; }

    public int? ArraySizeOf { get; init; }

    public bool IsArray => ArraySizeOf > 0;

    public override string ToString()
    {
        return string.IsNullOrEmpty(Name) ? string.Empty : Name;
    }

    public bool Equals(CSharpType? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        var isEqual = Name == other.Name &&
                      SizeOf == other.SizeOf &&
                      AlignOf == other.AlignOf &&
                      ArraySizeOf == other.ArraySizeOf;
        return isEqual;
    }

    public override int GetHashCode()
    {
        var hashCode = HashCode.Combine(Name, SizeOf, AlignOf, ArraySizeOf);
        return hashCode;
    }
}
