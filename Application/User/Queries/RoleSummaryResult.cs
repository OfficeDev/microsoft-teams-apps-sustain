// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application
{
    public class RoleSummaryResult : IMapFrom<Role>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
