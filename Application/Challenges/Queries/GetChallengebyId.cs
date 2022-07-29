// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;
public record GetChallengebyIdWithPaginationQuery : IRequest<PaginatedList<ChallengeSummaryResult>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int Id { get; init; }
    public bool ForManagement { get; set; } = false;
}

class GetChallengebyIdWithPaginationQueryValidator : AbstractValidator<GetChallengebyIdWithPaginationQuery>
{
    public GetChallengebyIdWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class GetChallengebyIdWithPaginationQueryHandler : IRequestHandler<GetChallengebyIdWithPaginationQuery, PaginatedList<ChallengeSummaryResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IIdentityService _identityService;
    private readonly IGraphService _graphService;


    public GetChallengebyIdWithPaginationQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper, 
        IBlobService blobService,
        IIdentityService identityService,
        IGraphService graphService
        )
    {
        _context = context;
        _mapper = mapper;
        _blobService=blobService;
        _identityService = identityService;
        _graphService = graphService;
    }

    public async Task<PaginatedList<ChallengeSummaryResult>> Handle(GetChallengebyIdWithPaginationQuery request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        
        var challenges = await _context.Challenges
            .Where(x => x.Id == request.Id)
            .ProjectTo<ChallengeSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        var challengesIds = challenges.Items.Select(x => x.Id).ToList();

        if (challenges.Items != null && challenges.Items.Count > 0)
        {
            string saslink = challenges.Items.FirstOrDefault().Thumbnail;
            string thumbnailWithSasToken = _blobService.GetSasLink(saslink);
            challenges.Items.FirstOrDefault().Thumbnail = thumbnailWithSasToken;
        }

        if (request.ForManagement)
        {
            return challenges;
        }

        var challengeRecord = await _context.ChallengeRecords
            .Include(x => x.Challenge)
            .Include(x => x.User)
            .Where(x => x.Challenge.Id == request.Id && x.User.Email.ToLower() == userEmail.ToLower())
            .OrderByDescending(x => x.Created)
            .ProjectTo<ChallengeRecordSummaryResult>(_mapper.ConfigurationProvider)
            .ToListAsync();

        challenges.Items.ForEach(x => x.ChallengeRecords = challengeRecord);

        if (challenges.Items != null && challenges.Items.Count > 0)
        {
            int status = -1;

            var challenge = challenges.Items.FirstOrDefault();
            var record = challengeRecord.FirstOrDefault();

            if (record != null)
            {
                status = (int) record.Status;
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

                if (record.Status == ChallengeRecordStatus.Abandoned) // if abandoned reset status
                {
                    status = -1;
                    challenge.ChallengeRecords = null;
                }
            }

            challenge.FinalStatus = status;
        }

        var relatedUsers = await _graphService.GetRelatedPeople(false);
        var relatedUsersEmails = relatedUsers.Select(x => x.EmailAddress).ToList();

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

            foreach (var userRecord in userRecords)
            {
                var tempUsers = targettedRelatedUsers.Where(x => userRecord.User.Email == x.EmailAddress).ToList();
                var challenge = challenges.Items.FirstOrDefault(x => x.Id == userRecord.Challenge.Id);

                if (tempUsers != null && challenge != null)
                {
                    tempUsers.ForEach(x =>
                    {
                        x.Status = (int)userRecord.Status;
                    });

                    challenge.RelatedUsers = tempUsers;
                }
            }
        }

        return challenges;
    }
}
