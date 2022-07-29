// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;
using Microsoft.Teams.Apps.Sustainability.Application.User.Queries;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI;

public class UserController : WebApiControllerBase
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    [HttpGet]
    [Route("me")]
    public async Task<ActionResult<UserSummaryResult>> Me()
    {
        return await Mediator.Send(new GetCurrentUserQuery() {});
    }

    [Authorize(Policy = "RequiredRoleUser,Admin")]
    [HttpGet]
    [Route("single")]
    public async Task<ActionResult<UserSummaryResult>> Single(string email)
    {
        return await Mediator.Send(new GetUserByEmailQuery() { Email = email });
    }

    [Authorize(Policy = "RequiredRoleUser,Admin")]
    [HttpGet]
    [Route("search")]
    public async Task<List<UserWithPhotoModel>> Search(string query)
    {
        return await Mediator.Send(new SearchUserQuery() { Query = query});
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpGet]
    public async Task<ActionResult<PaginatedList<UserSummaryResult>>> Get(int pageNumber = 1, int pagesize = 10, string? search = null, string? role = null)
    {
        var request = new GetUsersPaginatedQuery()
        {
            PageNumber = pageNumber,
            PageSize = pagesize,
            Role = role,
            Search = search
        };
        return await Mediator.Send(request);
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpPost]
    public async Task<ActionResult<int>> Post(CreateUserCommand request)
    {
        return await Mediator.Send(request);
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpPut]
    public async Task<ActionResult<int>> Put(UpdateUserCommand request)
    {
        return await Mediator.Send(request);
    }
    
    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpDelete]
    public async Task<ActionResult<int>> Delete(string email)
    {
        return await Mediator.Send(new DeleteUserCommand() { Email= email });
    }
}
