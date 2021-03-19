// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory (https://github.com/lithiumtoast/c2cs) for full license information.

namespace C2CS.CSharp
{
    public record CSharpCommon
    {
        public readonly string Name;
        public readonly string OriginalCodeLocationComment;

        public CSharpCommon(
            string name,
            string originalLocationComment)
        {
            Name = name;
            OriginalCodeLocationComment = originalLocationComment;
        }

        // Required for debugger string with records
        // ReSharper disable once RedundantOverriddenMember
        public override string ToString()
        {
            return $"{Name} {OriginalCodeLocationComment}";
        }
    }
}
