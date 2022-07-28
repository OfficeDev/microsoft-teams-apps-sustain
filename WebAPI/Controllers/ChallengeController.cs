// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI;

public class ChallengeController : WebApiControllerBase
{
    [Authorize(Policy = "RequiredRoleUser,Admin")]
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ChallengeSummaryResult>>> Get(int pageNumber = 1, int pageSize = 10, int? status = null)
    {
        return await Mediator.Send(new GetChallengesWithPaginationQuery() { PageNumber = pageNumber, PageSize = pageSize, Status = status});
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpGet]
    [Route("management")]
    public async Task<ActionResult<PaginatedList<ChallengeSummaryResult>>> GetAll(int pageNumber = 1, bool isArchived = false, int pagesize = 10, string? keyword = null)
    {
        return await Mediator.Send(new GetAllChallengesQuery() { PageNumber = pageNumber, PageSize = pagesize, Keyword = keyword, isArchived = isArchived });
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpPost]
    public async Task<ActionResult<int>> Post()
    {
        var request = new CreateChallengeCommand();

        var Title = Request.Form.FirstOrDefault(x => x.Key == "Title").Value;
        var IsPinned = Request.Form.FirstOrDefault(x => x.Key == "IsPinned").Value;
        var Recurrence = Request.Form.FirstOrDefault(x => x.Key == "Recurrence").Value;
        var Points = Request.Form.FirstOrDefault(x => x.Key == "Points").Value;
        var Description = Request.Form.FirstOrDefault(x => x.Key == "Description").Value;
        var ActiveUntil = Request.Form.FirstOrDefault(x => x.Key == "ActiveUntil").Value;
        var FocusArea = Request.Form.FirstOrDefault(x => x.Key == "FocusArea").Value;
        var AdditionalResources = Request.Form.FirstOrDefault(x => x.Key == "AdditionalResources").Value;

        request.Title = Title;
        request.IsPinned = bool.Parse(IsPinned);
        request.Recurrence = (Domain.ChallengeRecurrence)int.Parse(Recurrence);
        request.Points = int.Parse(Points);
        request.Description = Description;
        request.ActiveUntil = DateTime.Parse(ActiveUntil);
        request.FocusArea = FocusArea;
        request.AdditionalResources = AdditionalResources;

        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
        {
            using (var stream = new MemoryStream())
            {
                stream.Position = 0;
                Request.Form.Files[0].CopyTo(stream);

                request.ThumbnailFile = stream;
                request.ThumbnailFilename = Request.Form.FirstOrDefault(x => x.Key == "ThumbnailFilename").Value;

                return await Mediator.Send(request);
            }

        }

        return await Mediator.Send(new CreateChallengeCommand() { });
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpPut]
    public async Task<ActionResult<int>> Put()
    {
        var request = new UpdateChallengeCommand();

        var Id = Request.Form.FirstOrDefault(x => x.Key == "Id").Value;
        var Title = Request.Form.FirstOrDefault(x => x.Key == "Title").Value;
        var IsPinned = Request.Form.FirstOrDefault(x => x.Key == "IsPinned").Value;
        var Recurrence = Request.Form.FirstOrDefault(x => x.Key == "Recurrence").Value;
        var Points = Request.Form.FirstOrDefault(x => x.Key == "Points").Value;
        var Description = Request.Form.FirstOrDefault(x => x.Key == "Description").Value;
        var ActiveUntil = Request.Form.FirstOrDefault(x => x.Key == "ActiveUntil").Value;
        var FocusArea = Request.Form.FirstOrDefault(x => x.Key == "FocusArea").Value;
        var AdditionalResources = Request.Form.FirstOrDefault(x => x.Key == "AdditionalResources").Value;

        request.Id = int.Parse(Id);
        request.Title = Title;
        request.IsPinned = bool.Parse(IsPinned);
        request.Recurrence = (Domain.ChallengeRecurrence) int.Parse(Recurrence);
        request.Points = int.Parse(Points);
        request.Description = Description;
        request.ActiveUntil = DateTime.Parse(ActiveUntil);
        request.FocusArea = FocusArea;
        request.AdditionalResources = AdditionalResources;

        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
        {
            using(var stream = new MemoryStream())
            {
                stream.Position = 0;
                Request.Form.Files[0].CopyTo(stream);

                request.ThumbnailFile = stream;
                request.ThumbnailFilename = Request.Form.FirstOrDefault(x => x.Key == "ThumbnailFilename").Value;

                return await Mediator.Send(request);
            }
            
        }

        return await Mediator.Send(request);
    }

    [Authorize(Policy = "RequiredRoleAdmin")]
    [HttpDelete]
    public async Task<ActionResult<int>> Delete(int id)
    {
        return await Mediator.Send(new DeleteChallengeCommand(id) { });
    }

    [Authorize(Policy = "RequiredRoleUser,Admin")]
    [HttpGet]
    [Route("GetChallengebyId")]
    public async Task<ActionResult<PaginatedList<ChallengeSummaryResult>>> GetChallengebyId(int id, bool? forManagement = false)
    {
        var request = new GetChallengebyIdWithPaginationQuery() { PageNumber = 1, PageSize = 10, Id = id };

        if (forManagement != null)
        {
            request.ForManagement = (bool) forManagement;
        }

        return await Mediator.Send(request);
    }

    [HttpGet]
    [Route("GetChallengeStatus")]
    public async Task<ActionResult<PaginatedList<ChallengeSummaryResultStatus>>> GetChallengeStatus(int id)
    {
        return await Mediator.Send(new GetChallengestatusQuery() { PageNumber = 1, PageSize = 10, Id = id });
    }

    [HttpGet]
    [Route("GetNumUsersAccepted")]
    public async Task<int> GetNumUsersAccepted(int id)
    {
        var count = await Mediator.Send(new GetNumUsersAcceptChallengeQuery() { Id = id });

        return count;
    }

    [HttpGet]
    [Route("GetNumUsersCompleted")]
    public async Task<int> GetNumUsersCompleted(int id)
    {
        var count = await Mediator.Send(new GetNumUsersCompletedChallengeQuery() { Id = id });

        return count;
    }

    [HttpGet]
    [Route("GetCompletionPercentage")]
    public async Task<double> GetCompletionPercentage(int id)
    {
        double completed_count = await GetNumUsersCompleted(id);
        var accepted_count = GetNumUsersAccepted(id).Result;
        var percent = (completed_count / accepted_count) * 100;

        return percent;
    }
}
