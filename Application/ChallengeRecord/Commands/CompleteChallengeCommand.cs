// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record CompleteChallengeCommand : IRequest<int>
{
    public int ChallengeId { get; set; }
}

class CompleteChallengeValidatorCommandValidator : AbstractValidator<CompleteChallengeCommand>
{
    public CompleteChallengeValidatorCommandValidator()
    {
        RuleFor(v => v.ChallengeId)
            .GreaterThan(0)
            .NotEmpty();
    }
}

class CompleteChallengeCommandHandler : IRequestHandler<CompleteChallengeCommand, int>
{

    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public CompleteChallengeCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(CompleteChallengeCommand request, CancellationToken cancellationToken)
    {
        int result = 0;

        var date = DateTime.UtcNow;

        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == userEmail.ToLower());

        var challenge = await _context.Challenges.FirstOrDefaultAsync(x => x.Id == request.ChallengeId);

        if (challenge != null && user != null)
        {
            var currentChallengeRecord = _context.ChallengeRecords
            .Include(x => x.Challenge)
            .Include(x => x.User)
            .Where(x => x.Challenge.Id == request.ChallengeId && x.User.Email.ToLower() == userEmail.ToLower())
            .OrderByDescending(x => x.Created)
            .ToList();

            var alreadyCompletedToday = currentChallengeRecord.Any(x => x.Created.Date == date.Date && x.Status == ChallengeRecordStatus.Completed);

            if (!alreadyCompletedToday)
            {

                var challengeRecord = new ChallengeRecord()
                {
                    User = user,
                    Challenge = challenge,
                    Status = ChallengeRecordStatus.Completed,
                    ChallengePoint = challenge.Points,
                    Created = date,
                    LastModified = DateTimeOffset.UtcNow.DateTime,
                    CreatedBy = user.Email,
                    LastModifiedBy = user.Email,
                };

                _context.ChallengeRecords.Add(challengeRecord);

                // get summary for today
                var challengeRecordSummary = await _context.ChallengeRecordSummaries
                     .OrderByDescending(x => x.Created)
                     .FirstOrDefaultAsync(
                         x => x.User.Id == user.Id
                     );

                // if there is a record today, add points
                if (challengeRecordSummary != null)
                {
                    var currentPoint = challengeRecordSummary.CurrentPoints;
                    challengeRecordSummary.CurrentPoints = currentPoint + challenge.Points;
                    challengeRecordSummary.LastModified = DateTimeOffset.UtcNow.DateTime;
                    challengeRecordSummary.LastModifiedBy = user.Email;

                    _context.ChallengeRecordSummaries.Update(challengeRecordSummary);
                }
                else
                {
                    // if there is no record for today, check for other past dates
                    var previousChallengeRecordSummary = await _context.ChallengeRecordSummaries
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefaultAsync(
                        x => x.User.Id == user.Id
                    );

                    int previousPoints = 0;

                    if (previousChallengeRecordSummary != null)
                    {
                        previousPoints = previousChallengeRecordSummary.CurrentPoints;
                    }

                    // combine previous points to current
                    int latestPoints = previousPoints + challenge.Points;

                    challengeRecordSummary = new ChallengeRecordSummary()
                    {
                        User = user,
                        CurrentPoints = latestPoints,
                        Created = DateTimeOffset.UtcNow.DateTime,
                        LastModified = DateTimeOffset.UtcNow.DateTime,
                        CreatedBy = user.Email,
                        LastModifiedBy = user.Email
                    };

                    _context.ChallengeRecordSummaries.Add(challengeRecordSummary);
                }

                result = await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return result;
    }

}
