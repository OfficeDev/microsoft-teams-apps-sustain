// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record CreateChallengeCommand : IRequest<int>
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

class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}

class CreateChallengeCommandHandler : IRequestHandler<CreateChallengeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBlobService _blobService;
    private readonly IIdentityService _identityService;

    public CreateChallengeCommandHandler(IApplicationDbContext context, IBlobService blobService, IIdentityService identityService)
    {
        _context = context;
        _blobService=blobService;
        _identityService = identityService;
    }

    public async Task<int> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        string link = await _blobService.UploadFile(request.ThumbnailFile, request.ThumbnailFilename);

        var entity = new Challenge
        {
            Title = request.Title,
            ActiveUntil = request.ActiveUntil,
            AdditionalResources = request.AdditionalResources,
            Created =  DateTime.UtcNow,
            CreatedBy = userEmail,
            Description = request.Description,
            FocusArea = request.FocusArea,
            IsActive = true,
            Points = request.Points,
            Recurrence = request.Recurrence,
            Thumbnail = link,
            IsPinned = request.IsPinned
        };

        entity.AddDomainEvent(new ChallengeCreatedEvent(entity));

        // pinned validation
        var pinnedChallenges = _context.Challenges.Where(x => x.IsPinned != null && (bool) x.IsPinned).ToList();
        if (pinnedChallenges.Count >= 4 && request.IsPinned != null && (bool)request.IsPinned)
        {
            throw new ChallengePinningException("Only 4 challenges can be pinned at once.");
        }

        _context.Challenges.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
