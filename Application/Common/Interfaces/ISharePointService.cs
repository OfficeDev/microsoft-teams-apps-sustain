// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public interface ISharePointService
{
    Task<List<SharePointNews>> GetRecentNews(int top, CancellationToken cancellationToken);
    Task<List<SharePointEvent>> GetUpcomingEvents(int top, CancellationToken cancellationToken);
}
