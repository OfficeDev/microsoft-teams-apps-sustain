// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record UpdateChallengeCommand : IRequest<int>
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public int Points { get; set; }
    public string? Thumbnail { get; set; }
    public string Description { get; set; } = "";
    public DateTimeOffset ActiveUntil { get; set; }
    public ChallengeRecurrence Recurrence { get; set; }
    public string? FocusArea { get; set; }
    public string? AdditionalResources { get; set; }
    public bool IsActive { get; set; } = true;
    public bool? IsPinned { get; set; }
    public int FinalStatus { get; set; }
    public Stream ThumbnailFile { get; set; } = null;
    public string ThumbnailFilename { get; set; } = "";
}

class UpdateChallengeCommandValidator : AbstractValidator<UpdateChallengeCommand>
{
    public UpdateChallengeCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}

class UpdateChallengeCommandHandler : IRequestHandler<UpdateChallengeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IBlobService _blobService;

    public UpdateChallengeCommandHandler(IApplicationDbContext context, IIdentityService identityService, IBlobService blobService)
    {
        _context = context;
        _identityService = identityService;
        _blobService = blobService;       
    }

    public async Task<int> Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        var challenge = await _context.Challenges.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (challenge == null)
        {
            throw new NotFoundException(nameof(Challenge), request.Id);
        }

        // pinned validation
        var pinnedChallenges = _context.Challenges.Where(x => x.IsPinned != null && (bool)x.IsPinned).ToList();
        var isIncluded = pinnedChallenges.Any(x => x.Id == request.Id);
        if (pinnedChallenges.Count >= 4 && request.IsPinned != null && (bool) request.IsPinned && !isIncluded)
        {
            
            throw new ChallengePinningException("Only 4 challenges can be pinned at once.");
        }

        string link = string.Empty;

        challenge.Title = request.Title;
        challenge.ActiveUntil = request.ActiveUntil;
        challenge.AdditionalResources = request.AdditionalResources;
        challenge.Description = request.Description;
        challenge.FocusArea = request.FocusArea;
        challenge.Points = request.Points;
        challenge.Recurrence = request.Recurrence;
        challenge.IsPinned = request.IsPinned;


        challenge.LastModified =  DateTime.UtcNow;
        challenge.LastModifiedBy= userEmail;


        if (request.ThumbnailFile != null)
        {
            link = await _blobService.UploadFile(request.ThumbnailFile, request.ThumbnailFilename);
            challenge.Thumbnail = link;
        }


        await _context.SaveChangesAsync(cancellationToken);

        return 1;
    }
}
