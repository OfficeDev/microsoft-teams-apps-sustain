// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetRecentNewsQuery : IRequest<List<SharePointNewsResult>>
{
    public int Top { get; init; } = 4;
}

class GetRecentNewsQueryValidator : AbstractValidator<GetRecentNewsQuery>
{
    public GetRecentNewsQueryValidator()
    {
        RuleFor(x => x.Top)
            .GreaterThanOrEqualTo(1).WithMessage("Top at least greater than or equal to 1.");
    }
}

class GetRecentNewsQueryHandler : IRequestHandler<GetRecentNewsQuery, List<SharePointNewsResult>>
{
    private readonly IMapper _mapper;
    private readonly ISharePointService _spo;

    public GetRecentNewsQueryHandler(IMapper mapper, ISharePointService spo)
    {
        _mapper = mapper;
        _spo = spo;
    }

    public async Task<List<SharePointNewsResult>> Handle(GetRecentNewsQuery request, CancellationToken cancellationToken)
    {
        var news = await _spo.GetRecentNews(request.Top, cancellationToken);
        var results = news.AsQueryable().ProjectTo<SharePointNewsResult>(_mapper.ConfigurationProvider).ToList();
        return results;
    }
}
