// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetAllChallengesQuery : IRequest<PaginatedList<ChallengeSummaryResult>>
{
    public bool isArchived { get; set; } = false;
    public string? Keyword { get; set; } = null;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

class GetChallengesQueryValidator : AbstractValidator<GetAllChallengesQuery>
{
    public GetChallengesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}


class GetAllChallengesQueryHandler : IRequestHandler<GetAllChallengesQuery, PaginatedList<ChallengeSummaryResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;

    public GetAllChallengesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IBlobService blobService)
    {
        _context = context;
        _mapper = mapper;
        _blobService = blobService;
    }

    public async Task<PaginatedList<ChallengeSummaryResult>> Handle(GetAllChallengesQuery request, CancellationToken cancellationToken)
    {
        var challengesQuery = _context.Challenges
            .OrderByDescending(c => c.IsPinned)
            .ThenByDescending(c => c.Created)
            .Where(x => x.IsActive);

        if (request.isArchived)
        {
            challengesQuery = challengesQuery.Where(x => x.ActiveUntil < DateTimeOffset.UtcNow);
        }

        if(!string.IsNullOrEmpty(request.Keyword))
        {
            challengesQuery = challengesQuery.Where(
                x => x.Title.ToLower().Contains(request.Keyword.ToLower()) || x.Description.ToLower().Contains(request.Keyword.ToLower())
            );
        }

        var resultSet = await challengesQuery.ProjectTo<ChallengeSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        resultSet = await GetSASToken(resultSet);

        return resultSet;
    }


    private async Task<PaginatedList<ChallengeSummaryResult>> GetSASToken(PaginatedList<ChallengeSummaryResult> challenges)
    {

        if (challenges.Items != null && challenges.Items.Count > 0)
        {
            challenges.Items.ForEach(x =>
            {
                string saslink = x.Thumbnail;

                if (!string.IsNullOrEmpty(saslink))
                {
                    string thumbnailWithSasToken = _blobService.GetSasLink(saslink);
                    x.Thumbnail = thumbnailWithSasToken;
                }

            });
        }

        return challenges;
    }
}
