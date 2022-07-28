// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    public class NewsController : WebApiControllerBase
    {
        [HttpGet("recent")]
        public async Task<ActionResult<List<SharePointNewsResult>>> GetRecentNews()
        {
            return await Mediator.Send(new GetRecentNewsQuery() { Top = 4 });
        }
    }
}