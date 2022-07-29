// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record SendChallengeNotificationCommand : IRequest
{
    public string Recipients { get; init; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public int ChallengeId { get; set; } = 0;
    public string AppId { get; set; }
    public string PageId { get; set; }
}

class SendChallengeNotificationCommandValidator : AbstractValidator<SendChallengeNotificationCommand>
{
    public SendChallengeNotificationCommandValidator()
    {
        RuleFor(v => v.Recipients).NotEmpty();
        RuleFor(v => v.Message).MaximumLength(150).WithMessage("Message cannot be greater than 150 characters.");
        RuleFor(v => v.Title).MaximumLength(50).WithMessage("Title cannot be greater than 50 characters.");
    }
}

class SendChallengeNotificationCommandHandler : IRequestHandler<SendChallengeNotificationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IGraphService _graphService;

    public SendChallengeNotificationCommandHandler(IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IGraphService graphService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _graphService = graphService;
    }

    public async Task<Unit> Handle(SendChallengeNotificationCommand request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        await _graphService.SendActivityFeedNotification(request.Recipients, request.Title, request.Message, request.AppId, request.PageId,  request.ChallengeId);

        return Unit.Value;
    }
}
