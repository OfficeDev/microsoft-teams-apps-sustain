// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain
{
    public class SiteConfig: BaseAuditableEntity
    {
        public SiteConfigServiceType ServiceType { get; set; }
        public string URI { get; set; } = "";
        public bool? IsEnabled { get; set; }
        public bool? IsNewsEnabled { get; set; }
        public bool? IsEventsEnabled { get; set; }
    }
}
