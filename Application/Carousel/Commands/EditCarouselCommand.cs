// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record EditCarouselCommand : IRequest<int>
{
    public int Id { get; set; }
    public string Link { get; set; } = "";
    public string Title { get; set; } = "";
    public string Thumbnail { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive { get; set; }
    public Stream Photo { get; set; } = null;
    public string PhotoFileName { get; set; } = "";
}

class EditCarouselCommandValidator : AbstractValidator<EditCarouselCommand>
{
    public EditCarouselCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(v => v.Description)
            .MaximumLength(255)
            .NotEmpty();
    }
}

class EditCarouselCommandHandler : IRequestHandler<EditCarouselCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IBlobService _blobService;

    public EditCarouselCommandHandler(IApplicationDbContext context, IIdentityService identityService, IBlobService blobService)
    {
        _context = context;
        _identityService = identityService;
        _blobService = blobService;
    }

    public async Task<int> Handle(EditCarouselCommand request, CancellationToken cancellationToken)
    {
        int result = 0;
        string userEmail = _identityService.CurrentUserEmail != null ? _identityService.CurrentUserEmail : "";
        var currDate = DateTime.UtcNow;

        var carousel = _context.Carousels.FirstOrDefault(x => x.Id == request.Id);

        if (carousel != null)
        {
            if (!string.IsNullOrEmpty(request.PhotoFileName))
            {
                await _blobService.RemoveFile(carousel.Thumbnail);

                string newLink = await _blobService.UploadFile(request.Photo, request.PhotoFileName);
                carousel.Thumbnail = newLink;
            }

            carousel.Title = request.Title;
            carousel.Description = request.Description;
            carousel.LastModified = currDate;
            carousel.LastModifiedBy = userEmail;
            carousel.Link = request.Link;

            _context.Carousels.Update(carousel);

            result = await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}