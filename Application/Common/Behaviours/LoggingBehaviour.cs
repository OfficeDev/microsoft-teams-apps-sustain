// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        string userId = _identityService.CurrentUserEmail ?? string.Empty;

        _logger.LogInformation("Request: {Name} {@UserId} {@Request}",
            requestName, userId, request);

        return Task.CompletedTask;
    }
}
