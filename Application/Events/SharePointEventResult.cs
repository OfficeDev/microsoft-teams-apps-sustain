// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class SharePointEventResult : IMapFrom<SharePointEvent>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Link { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsAllDayEvent { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SharePointEvent, SharePointEventResult>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.Location))
            .ForMember(d => d.Link, o => o.MapFrom(s => s.Path))
            .ForMember(d => d.Start, o => o.MapFrom(s => s.EventDate))
            .ForMember(d => d.End, o => o.MapFrom(s => s.EndDate))
            .ForMember(d => d.IsAllDayEvent, o => o.MapFrom(s => s.IsAllDayEvent));
    }
}
