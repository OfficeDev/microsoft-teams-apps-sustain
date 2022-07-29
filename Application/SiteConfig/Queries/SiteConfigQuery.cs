// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;
public record SiteConfigQuery : IRequest<PaginatedList<SiteConfigSummaryResult>>
{
    public int? ServiceType { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}


class SiteConfigQueryValidator: AbstractValidator<SiteConfigQuery>
{
    public SiteConfigQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class SiteConfigQueryHandler: IRequestHandler<SiteConfigQuery, PaginatedList<SiteConfigSummaryResult>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public SiteConfigQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper=mapper;     
    }

    public async Task<PaginatedList<SiteConfigSummaryResult>> Handle(SiteConfigQuery request, CancellationToken cancellationToken)
    {
        SiteConfigServiceType serviceType = (SiteConfigServiceType)request.ServiceType;

        var result = await _dbContext
            .SiteConfigs
            .Where(x => x.ServiceType == serviceType)
            .ProjectTo<SiteConfigSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        
        return result;
    }
}
    
