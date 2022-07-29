// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain
{
    public class Carousel: BaseAuditableEntity
    {
        public string Link { get; set; } = "";
        public string Title { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
