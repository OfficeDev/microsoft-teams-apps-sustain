// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record AcceptChallengeCommand : IRequest<int>
{
    public int ChallengeId { get; set; }
}

class AcceptChallengeValidatorCommandValidator : AbstractValidator<AcceptChallengeCommand>
{
    public AcceptChallengeValidatorCommandValidator()
    {
        RuleFor(v => v.ChallengeId)
            .GreaterThan(0)
            .NotEmpty();
    }
}

class AcceptChallengeCommandHandler : IRequestHandler<AcceptChallengeCommand, int>
{

    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public AcceptChallengeCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(AcceptChallengeCommand request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        var challengeRecord = new List<ChallengeRecordSummaryResult>();

        var challenge = _context.Challenges.FirstOrDefault(x => x.Id == request.ChallengeId);

        if (challenge != null)
        {
            Domain.User? userObject = null;

            userObject = _context.Users.FirstOrDefault(x => x.Email.ToLower() == userEmail.ToLower());

            if (userObject == null)
            {
                var saveUser = _context.Users.Add(new Domain.User()
                {
                    Username = "",
                    Email = userEmail,
                    Created = DateTimeOffset.UtcNow.DateTime,
                    LastModified = DateTimeOffset.UtcNow.DateTime,
                });

                userObject = saveUser.Entity;
            }

            
            var record = new ChallengeRecord()
            {
                Challenge = challenge,
                ChallengePoint = challenge.Points,
                User = userObject,
                Status = ChallengeRecordStatus.Accepted,
                    
                CreatedBy = userEmail,
                Created = DateTimeOffset.UtcNow.DateTime,
                LastModifiedBy = userEmail,
                LastModified = DateTime.UtcNow
            };

            await _context.ChallengeRecords.AddAsync(record);

        }

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result;
    }

}