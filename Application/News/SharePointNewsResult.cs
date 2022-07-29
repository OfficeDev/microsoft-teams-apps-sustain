// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class SharePointNewsResult : IMapFrom<SharePointNews>
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Link { get; set; } = "";
    public string ThumbnailUrl { get; set; } = "";
    public int Views { get; set; }
    public string SiteTitle { get; set; } = "";
    public string SiteLogoUrl { get; set; } = "";
    public string SeeAllUrl { get; set; } = "";
    public DateTime LastModified { get; set; }
    public DateTime FirstPublished { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SharePointNews, SharePointNewsResult>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.Link, o => o.MapFrom(s => s.Path))
            .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => s.PictureThumbnailURL))
            .ForMember(d => d.Views, o => o.MapFrom(s => s.ViewsRecent))
            .ForMember(d => d.SiteTitle, o => o.MapFrom(s => s.SiteTitle))
            .ForMember(d => d.SiteLogoUrl, o => o.MapFrom(s => s.SiteLogo))
            .ForMember(d => d.SeeAllUrl, o => o.MapFrom(s => s.SeeAllUrl))
            .ForMember(d => d.LastModified, o => o.MapFrom(s => s.LastModifiedTime))
            .ForMember(d => d.FirstPublished, o => o.MapFrom(s => s.FirstPublishedDate));
    }
}
