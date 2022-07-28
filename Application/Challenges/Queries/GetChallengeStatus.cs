// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;
public record GetChallengestatusQuery : IRequest<PaginatedList<ChallengeSummaryResultStatus>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int Id { get; init; }
}

class GetChallengestatusQueryValidator : AbstractValidator<GetChallengestatusQuery>
{
    public GetChallengestatusQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class GetChallengestatusQueryHandler : IRequestHandler<GetChallengestatusQuery, PaginatedList<ChallengeSummaryResultStatus>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public GetChallengestatusQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
    }

    public async Task<PaginatedList<ChallengeSummaryResultStatus>> Handle(GetChallengestatusQuery request, CancellationToken cancellationToken)
    {
        var userEmail = _identityService.CurrentUserEmail;
        var user = await _context.Users
            .Where(u => u.Email == userEmail)
            .FirstOrDefaultAsync();

        var result =  await _context.ChallengeRecords
            .Where(x => x.Challenge.Id == request.Id && x.User.Id == user.Id)
            .ProjectTo<ChallengeSummaryResultStatus>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}

