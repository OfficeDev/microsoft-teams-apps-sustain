// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application.Dashboard.Queries;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI.Controllers
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    public class DashboardController : WebApiControllerBase
    {
        [HttpGet]
        public async Task<DashboardDetails> Get()
        {
            return await Mediator.Send(new GetUserDashboardDetails() { });
        }
    }
}
