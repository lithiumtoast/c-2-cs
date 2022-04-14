// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace C2CS.Feature.WriteCodeCSharp.Data.Model;

public sealed class CSharpConstant : CSharpNode
{
    public readonly string Type;

    public readonly string Value;

    public CSharpConstant(
        TargetPlatform platform,
        string name,
        string codeLocationComment,
        int? sizeOf,
        string type,
        string value)
        : base(platform, name, codeLocationComment, sizeOf)
    {
        Type = type;
        Value = value;
    }

    public override bool Equals(CSharpNode? other)
    {
        if (!base.Equals(other) || other is not CSharpConstant other2)
        {
            return false;
        }

        return Type == other2.Type &&
               Value == other2.Value;
    }
}
