// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;
namespace Microsoft.Teams.Apps.Sustainability.Application;

public record AbandonChallengeCommand : IRequest<int>
{
    public int ChallengeId { get; set; }
}

class LeaveChallengeValidatorCommandValidator : AbstractValidator<AbandonChallengeCommand>
{
    public LeaveChallengeValidatorCommandValidator()
    {
        RuleFor(v => v.ChallengeId)
            .GreaterThan(0)
            .NotEmpty();
    }
}

class LeaveChallengeCommandHandler : IRequestHandler<AbandonChallengeCommand, int>
{

    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    
    public LeaveChallengeCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(AbandonChallengeCommand request, CancellationToken cancellationToken)
    {
        int result = 0;
        
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == userEmail.ToLower());

        var challenge = _context.Challenges.FirstOrDefault(x => x.Id == request.ChallengeId);

        if (challenge != null && user != null)
        {
            var challengeRecord = new ChallengeRecord()
            {
                Challenge = challenge,
                ChallengePoint = 0,
                Created = DateTimeOffset.UtcNow.DateTime,
                CreatedBy = user.Email,
                LastModified = DateTimeOffset.UtcNow.DateTime,
                LastModifiedBy = user.Email,
                Status = ChallengeRecordStatus.Abandoned,
                User = user
            };

            _context.ChallengeRecords.Add(challengeRecord);

            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

}

