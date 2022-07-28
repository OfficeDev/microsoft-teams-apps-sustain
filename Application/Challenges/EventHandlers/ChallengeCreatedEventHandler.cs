// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

class ChallengeCreatedEventHandler : INotificationHandler<ChallengeCreatedEvent>
{
    private readonly ILogger<ChallengeCreatedEventHandler> _logger;

    public ChallengeCreatedEventHandler(ILogger<ChallengeCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ChallengeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
