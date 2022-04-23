// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace C2CS.Tests.Common.Data.Model.CSharp;

[PublicAPI]
public class CSharpTestStructField
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type_name")]
    public string TypeName { get; set; } = string.Empty;

    [JsonPropertyName("offset_of")]
    public int OffsetOf { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
