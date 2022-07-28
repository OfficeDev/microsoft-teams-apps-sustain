// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record DeleteCarouselCommand : IRequest<int>
{
    public int Id { get; set; }
}

class DeleteCarouselCommandValidator : AbstractValidator<DeleteCarouselCommand>
{
    public DeleteCarouselCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();
    }
}

class DeleteCarouselCommandHandler : IRequestHandler<DeleteCarouselCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IBlobService _blobService;

    public DeleteCarouselCommandHandler(IApplicationDbContext context, IIdentityService identityService, IBlobService blobService)
    {
        _context = context;
        _identityService = identityService;
        _blobService = blobService;
    }

    public async Task<int> Handle(DeleteCarouselCommand request, CancellationToken cancellationToken)
    {
        string userEmail = _identityService.CurrentUserEmail != null ? _identityService.CurrentUserEmail : "";

        var currDate = DateTime.UtcNow;

        var carousel = _context.Carousels.FirstOrDefault(x => x.Id == request.Id);

        if (carousel != null)
        {
            await _blobService.RemoveFile(carousel.Thumbnail);
            _context.Carousels.Remove(carousel);
        }

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}