// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain
{
    public class DashboardDetails : BaseAuditableEntity
    {
        public string? UserName { get; set; }
        public int CurrentPoints { get; set; }
        public string? CurrentRankLabel { get; set; }
        public string? NextRankLabel { get; set; }
        public string? RemainingPoints { get; set; }
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public List<DashboardRank>? DashboardRanks { get; set; }
    }
}
