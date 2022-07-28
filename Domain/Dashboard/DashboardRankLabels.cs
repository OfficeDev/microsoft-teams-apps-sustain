// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class DashboardRankLabels : BaseAuditableEntity
{
    public string? id { get; set; }
    public string? Label { get; set; }
    public int Score { get; set; }
}
