// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;

public record GetNumUsersAcceptChallengeQuery : IRequest<int>
{
    public int Id { get; init; }
}

class GetNumUsersAcceptChallengeQueryValidator : AbstractValidator<GetNumUsersAcceptChallengeQuery>
{
    public GetNumUsersAcceptChallengeQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");
    }
}

class GetNumUsersAcceptChallengeQueryHandler : IRequestHandler<GetNumUsersAcceptChallengeQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNumUsersAcceptChallengeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(GetNumUsersAcceptChallengeQuery request, CancellationToken cancellationToken)
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
            )
            .ToList()
            .Count();

        return count;
    }
}
