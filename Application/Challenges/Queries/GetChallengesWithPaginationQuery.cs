// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetChallengesWithPaginationQuery : IRequest<PaginatedList<ChallengeSummaryResult>>
{
    public int? Status { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

class GetChallengesWithPaginationQueryValidator : AbstractValidator<GetChallengesWithPaginationQuery>
{
    public GetChallengesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class GetChallengesWithPaginationQueryHandler : IRequestHandler<GetChallengesWithPaginationQuery, PaginatedList<ChallengeSummaryResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IBlobService _blobService;
    private readonly IGraphService _graphService;

    public GetChallengesWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IBlobService blobService,
        IGraphService graphService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _blobService = blobService;
        _graphService = graphService;
    }

    public async Task<PaginatedList<ChallengeSummaryResult>> Handle(GetChallengesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        int userId = 0;

        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail)  ? _identityService.CurrentUserEmail : "" ;

        var userObject = _context.Users.FirstOrDefault(x => x.Email == userEmail);

        userId = userObject != null ? userObject.Id : 0;

        var challengeQuery = _context.Challenges
            .Where(
                x => x.IsActive
                && x.ActiveUntil >= DateTimeOffset.UtcNow
            );

        // request status filter
        if (request.Status != null && request.Status > -1)
        {
            challengeQuery = challengeQuery
            .Where(
                x => // should have at least 1 record
                    x.ChallengeRecords
                    .OrderByDescending(cr => cr.Created)
                    .FirstOrDefault(cr => cr.User.Id == userId) != null
                    // latest record should not be abandoned
                    && x.ChallengeRecords
                    .OrderByDescending(cr => cr.Created)
                    .FirstOrDefault(cr => cr.User.Id == userId).Status != ChallengeRecordStatus.Abandoned
                    && (
                        x.Recurrence == ChallengeRecurrence.Daily ? // if daily challange
                            (
                                (ChallengeRecordStatus)request.Status == ChallengeRecordStatus.Completed ? 
                                // and filter is completed
                                // there should be a record for today that is completed
                                (
                                    x.ChallengeRecords
                                    .OrderByDescending(cr => cr.Created)
                                    .FirstOrDefault(
                                        cr => cr.User.Id == userId 
                                        && cr.Status == ChallengeRecordStatus.Completed
                                        && cr.Created.Date == DateTime.UtcNow.Date
                                    ) 
                                    != null
                                )
                                : 
                                (ChallengeRecordStatus)request.Status == ChallengeRecordStatus.Accepted ?
                                // and filter is accepted
                                // there should not be a record for today that is completed
                                (
                                    x.ChallengeRecords
                                    .OrderByDescending(cr => cr.Created)
                                    .FirstOrDefault(cr => cr.User.Id == userId
                                        && cr.Status == ChallengeRecordStatus.Completed
                                        && cr.Created.Date == DateTime.UtcNow.Date
                                    ) == null
                                )
                                : true
                            ) :
                            // should match the criteria of status filter for normal challenges (not daily)
                            x.ChallengeRecords
                            .OrderByDescending(cr => cr.Created)
                            .FirstOrDefault(cr => cr.User.Id == userId).Status == (ChallengeRecordStatus)request.Status
                    )
                );
        }
        
        // paginated view
        var challenges = await challengeQuery
            .OrderByDescending(x => x.IsPinned)
            .ThenBy(x => x.Created)
            .ProjectTo<ChallengeSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // get statuses
        challenges = await GetCurrentStatuses(challenges);

        // get related users
        challenges = await GetRelatedUsers(challenges);

        // get sas token
        challenges = await GetSASToken(challenges);

        return challenges;
    }

    private async Task<PaginatedList<ChallengeSummaryResult>> GetCurrentStatuses(PaginatedList<ChallengeSummaryResult> challenges) 
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        
        var challengesIds = challenges.Items.Select(x => x.Id).ToList();

        // get statuses of current user
        var challengesResults = await _context.ChallengeRecords
            .Include(x => x.Challenge)
            .Include(x => x.User)
            .Where(
                x => challengesIds.Contains(x.Challenge.Id) && x.User.Email == userEmail
            )
            .OrderByDescending(x => x.Created)
            .ToListAsync();

        foreach(var challenge in challenges.Items)
        {
            challenge.ChallengeRecords = null;
            int status = -1;

            var record = challengesResults.OrderByDescending(cr => cr.Created).FirstOrDefault(x => challenge.Id == x.Challenge.Id);

            if (record != null)
            {
                status = (int) record.Status;

                // check for accepted
                if (
                    challenge.Recurrence == 0 // if daily or recurring
                    && record.Status == ChallengeRecordStatus.Completed // and status is completed
                    && record.Created.Date != DateTimeOffset.UtcNow.Date // but not today
                )
                {
                    // reset to accepted
                    record.Status = ChallengeRecordStatus.Accepted;
                    status = 0;
                }

                // assign records
                List<ChallengeRecord> dbRecords = new List<ChallengeRecord> { record };
                
                var recordSummary = _mapper.ProjectTo<ChallengeRecordSummaryResult>(dbRecords.AsQueryable()).ToList();
                challenge.ChallengeRecords = recordSummary;

                if (record.Status == ChallengeRecordStatus.Abandoned) // if abandoned set to null
                {
                    status = -1;
                    challenge.ChallengeRecords = null;
                }
            }

            challenge.FinalStatus = status;
        }

        return challenges;
    }

    private async Task<PaginatedList<ChallengeSummaryResult>> GetRelatedUsers(PaginatedList<ChallengeSummaryResult> challenges)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        var relatedUsers = await _graphService.GetRelatedPeople(false);
        var relatedUsersEmails = relatedUsers.Select(x => x.EmailAddress).ToList();

        var challengesIds = challenges.Items.Select(x => x.Id).ToList();

        var userRecords = _context.ChallengeRecords
            .Include(x => x.User)
            .Include(x => x.Challenge)
            .Where(
                x => relatedUsersEmails.Contains(x.User.Email)
                && challengesIds.Contains(x.Challenge.Id)
                && x.Status != ChallengeRecordStatus.Abandoned
                && x.User.Email != userEmail
            ).ToList();

        // filter related users with records and get photos

        if (userRecords != null && relatedUsers != null)
        {
            var relatedUsersWithRecord = userRecords.Select(x => x.User.Email).ToList();
            var targettedRelatedUsers = relatedUsers.Where(x => relatedUsersWithRecord.Contains(x.EmailAddress)).ToList();
            targettedRelatedUsers = await _graphService.GetUserPhotosBulk(targettedRelatedUsers);

            foreach(var userRecord in userRecords)
            {
                var users = targettedRelatedUsers.Where(x => userRecord.User.Email == x.EmailAddress).ToList();
                var challenge = challenges.Items.FirstOrDefault(x => x.Id == userRecord.Challenge.Id);

                if (users != null && challenge != null)
                {
                    challenge.RelatedUsers = users;
                }
            }
        }
        
        return challenges;
    }

   
    private async Task<PaginatedList<ChallengeSummaryResult>> GetSASToken(PaginatedList<ChallengeSummaryResult> challenges)
    {

        if (challenges.Items != null && challenges.Items.Count > 0)
        {
            challenges.Items.ForEach(x =>
            {
                string saslink = x.Thumbnail;
                
                if (!string.IsNullOrEmpty(saslink))
                {
                    string thumbnailWithSasToken = _blobService.GetSasLink(saslink);
                    x.Thumbnail = thumbnailWithSasToken;
                }
                
            });
        }

        return challenges;
    }

}
