// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record AddCarouselCommand: IRequest<int>
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

class AddCarouselCommandValidator: AbstractValidator<AddCarouselCommand>
{
    public AddCarouselCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(v => v.Description)
            .MaximumLength(255)
            .NotEmpty();
    }
}

class AddCarouselCommandHandler : IRequestHandler<AddCarouselCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IBlobService _blobService;

    public AddCarouselCommandHandler(IApplicationDbContext context, IIdentityService identityService, IBlobService blobService)
    {
        _context = context;
        _identityService = identityService;
        _blobService = blobService;       
    }

    public async Task<int> Handle(AddCarouselCommand request, CancellationToken cancellationToken)
    {
        int result = 0;

        var carouselRecord = _context.Carousels.ToList();

        //set limit to 5
        if (carouselRecord.Count < 5)
        {
            string userEmail = _identityService.CurrentUserEmail != null ? _identityService.CurrentUserEmail : "";

            var currDate = DateTime.UtcNow;

            string link = await _blobService.UploadFile(request.Photo, request.PhotoFileName);

            Carousel carousel = new Carousel()
            {
                Title = request.Title,
                Description = request.Description,
                Created = currDate,
                CreatedBy = userEmail,
                IsActive = true,
                LastModified = currDate,
                LastModifiedBy = userEmail,
                Thumbnail = link,
                Link = request.Link
            };

            _context.Carousels.Add(carousel);

            result = await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}