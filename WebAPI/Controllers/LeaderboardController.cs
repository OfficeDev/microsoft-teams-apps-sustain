// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Application.Leaderboard.Queries;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI.Controllers
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    public class LeaderboardController : WebApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<LeaderboardResult>>> Get(int count, bool filteredByOrg)
        {
            return await Mediator.Send(new GetLeaderboardQuery() { PageNumber = 1, PageSize = count, FilteredByOrg = filteredByOrg });
        }
    }
}
