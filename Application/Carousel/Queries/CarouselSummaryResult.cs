// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class CarouselSummaryResult : IMapFrom<Carousel>
{
    public int Id { get; set; }
    public string Link { get; set; } = "";
    public string Title { get; set; } = "";
    public string Thumbnail { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive { get; set; }
    public string Filename { get; set; } = "";
}
