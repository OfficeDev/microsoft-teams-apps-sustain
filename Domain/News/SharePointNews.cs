// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class SharePointNews : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Path { get; set; }
    public string? PictureThumbnailURL { get; set; }
    public int ViewsRecent { get; set; }
    public string? SiteTitle { get; set; }
    public string? SiteLogo { get; set; }
    public string? SeeAllUrl { get; set; }
    public DateTime LastModifiedTime { get; set; }
    public DateTime FirstPublishedDate { get; set; }
}
