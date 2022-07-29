// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetUpcomingEventsQuery : IRequest<SharePointEventResultSummary>
{
    public int Top { get; init; } = 2;
}

class GetUpcomingEventsQueryValidator : AbstractValidator<GetUpcomingEventsQuery>
{
    public GetUpcomingEventsQueryValidator()
    {
        RuleFor(x => x.Top)
            .GreaterThanOrEqualTo(1).WithMessage("Top at least greater than or equal to 1.");
    }
}

class GetUpcomingEventsQueryHandler : IRequestHandler<GetUpcomingEventsQuery, SharePointEventResultSummary>
{
    private readonly IMapper _mapper;
    private readonly ISharePointService _spo;
    private readonly IApplicationDbContext _context;

    public GetUpcomingEventsQueryHandler(IMapper mapper, ISharePointService spo, IApplicationDbContext context)
    {
        _mapper = mapper;
        _spo = spo;
        _context = context;
    }

    public async Task<SharePointEventResultSummary> Handle(GetUpcomingEventsQuery request, CancellationToken cancellationToken)
    {
        string eventsViewAllLink = string.Empty;

        var spSiteConfig = _context.SiteConfigs.FirstOrDefault(x => x.ServiceType == SiteConfigServiceType.SharePoint);

        if (spSiteConfig != null)
        {
            string spSite= spSiteConfig.URI;
            

            var startDate = DateTimeOffset.UtcNow;
            var endDate = DateTimeOffset.UtcNow.AddMonths(6);

            eventsViewAllLink = $"{spSite}/_layouts/15/Events.aspx?InstanceId=00000000-0000-0000-0000-000000000000&StartDate={startDate.ToString("d")}&EndDate={endDate.ToString("d")}&AudienceTarget=true";
        }

        var events = await _spo.GetUpcomingEvents(request.Top, cancellationToken);
        var results = events.AsQueryable().ProjectTo<SharePointEventResult>(_mapper.ConfigurationProvider).ToList();

        SharePointEventResultSummary resultSet = new SharePointEventResultSummary()
        {
            Items = results, 
            EventsViewAllLink = eventsViewAllLink
        };


        return resultSet;
    }
}
