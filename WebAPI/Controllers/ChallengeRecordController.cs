// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI;

[Authorize(Policy = "RequiredRoleUser,Admin")]
public class ChallengeRecordController: WebApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Post(AcceptChallengeCommand request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut]
    public async Task<ActionResult<int>> Put(CompleteChallengeCommand request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete]
    public async Task<ActionResult<int>> Delete(AbandonChallengeCommand request)
    {
        return await Mediator.Send(request);
    }
}
