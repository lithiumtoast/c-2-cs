// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;

namespace C2CS.Feature.BindgenCSharp.Data.Model;

public sealed class CSharpFunction : CSharpNode
{
    public readonly CSharpFunctionCallingConvention CallingConvention;

    public readonly ImmutableArray<CSharpFunctionParameter> Parameters;

    public readonly CSharpType ReturnType;

    public CSharpFunction(
        string name,
        string codeLocationComment,
        int? sizeOf,
        CSharpFunctionCallingConvention callingConvention,
        CSharpType returnType,
        ImmutableArray<CSharpFunctionParameter> parameters)
        : base(name, codeLocationComment, sizeOf)
    {
        CallingConvention = callingConvention;
        ReturnType = returnType;
        Parameters = parameters;
    }

    public override bool Equals(CSharpNode? other)
    {
        if (!base.Equals(other) || other is not CSharpFunction other2)
        {
            return false;
        }

        return CallingConvention == other2.CallingConvention &&
               ReturnType == other2.ReturnType &&
               Parameters.SequenceEqual(other2.Parameters);
    }
}
