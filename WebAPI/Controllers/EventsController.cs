// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    public class EventsController : WebApiControllerBase
    {
        [HttpGet("upcoming")]
        public async Task<ActionResult<SharePointEventResultSummary>> GetUpcomingEvents()
        {
            return await Mediator.Send(new GetUpcomingEventsQuery() { Top = 2 });
        }
    }
}