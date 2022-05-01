// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using C2CS.Feature.ReadCodeC.Data;
using C2CS.Feature.ReadCodeC.Data.Model;
using static bottlenoselabs.clang;

namespace C2CS.Feature.ReadCodeC.Domain.ExploreCode;

internal sealed class ExplorerNode
{
    public readonly CKind Kind;
    public readonly CLocation Location;
    public readonly ExplorerNode? Parent;
    public readonly CXCursor Cursor;
    public readonly CXType Type;
    public readonly CXType ContainerType;
    public readonly string? CursorName;
    public readonly string? TypeName;

    public ExplorerNode(
        CKind kind,
        CLocation location,
        ExplorerNode? parent,
        CXCursor cursor,
        CXType type,
        CXType containerType,
        string? cursorName,
        string? typeName)
    {
        Kind = kind;
        Location = location;
        Parent = parent;
        Cursor = cursor;
        Type = type;
        ContainerType = containerType;
        CursorName = cursorName;
        TypeName = typeName;
    }

    public override string ToString()
    {
        return Location.ToString();
    }
}
