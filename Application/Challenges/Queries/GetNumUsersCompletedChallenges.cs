// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;

public record GetNumUsersCompletedChallengeQuery : IRequest<int>
{
    public int Id { get; init; }
}

class GetNumUsersCompletedChallengeQueryValidator : AbstractValidator<GetNumUsersCompletedChallengeQuery>
{
    public GetNumUsersCompletedChallengeQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

    }
}

class GetNumUsersCompletedChallengeQueryHandler : IRequestHandler<GetNumUsersCompletedChallengeQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNumUsersCompletedChallengeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(GetNumUsersCompletedChallengeQuery request, CancellationToken cancellationToken)
    {
        var challenge = _context.Challenges.FirstOrDefault(x => x.Id == request.Id);

        var count = _context.Users
            .Include("ChallengeRecords")
            .Include("ChallengeRecords.Challenge")
            .Where(
                x => x.ChallengeRecords != null
                && x.ChallengeRecords
                .OrderByDescending(cr => cr.Created)
                .FirstOrDefault(cr => cr.Challenge.Id == request.Id) != null
                && x.ChallengeRecords
                .OrderByDescending(cr => cr.Created)
                .FirstOrDefault(cr => cr.Challenge.Id == request.Id).Status != ChallengeRecordStatus.Abandoned
                && x.ChallengeRecords
                .OrderByDescending(cr => cr.Created)
                .FirstOrDefault(cr => cr.Challenge.Id == request.Id).Status == ChallengeRecordStatus.Completed
            )
            .ToList()
            .Count();

        return count;
    }
}
