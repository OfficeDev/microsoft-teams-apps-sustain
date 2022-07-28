// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain
{
    public class DashboardRank : BaseAuditableEntity
    {
        public string? Label { get; set; }
        public string? Score { get; set; }
        public bool IsActive{ get; set; }
    }
}
