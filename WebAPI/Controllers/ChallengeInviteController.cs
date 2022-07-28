// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI
{
    [Authorize]
    public class ChallengeInviteController : WebApiControllerBase
    {

        [HttpPost]
        [Route("SendChallengeNotification")]
        public async Task<Unit> SendChallengeNotification(SendChallengeNotificationCommand request)
        {
            await Mediator.Send(request);

            return Unit.Value;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<List<UserWithPhotoModel>> GetUsers()
        {
            var results = await Mediator.Send(new GetUsersToInviteQuery() { });

            return results;
        }
    }
}