// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class SiteConfigSummaryResult : IMapFrom<SiteConfig>
{
    public int ServiceType { get; set; }
    public string URI { get; set; } = "";
    public bool? IsEnabled { get; set; }
    public bool? IsNewsEnabled { get; set; }
    public bool? IsEventsEnabled { get; set; }
}
