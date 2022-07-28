// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;


namespace Microsoft.Teams.Apps.Sustainability.WebAPI
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    public class SiteConfigController : WebApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<SiteConfigSummaryResult>>> Get(int pageNumber = 1, int pageSize = 10, int serviceType = 1)
        {
            return await Mediator.Send(new SiteConfigQuery() { PageNumber = pageNumber, PageSize = pageSize, ServiceType = serviceType});
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(UpdateSiteConfigCommand request)
        {
            return await Mediator.Send(request);
        }

    }
}
