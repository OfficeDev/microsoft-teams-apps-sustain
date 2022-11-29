// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Application.Leaderboard.Queries;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application.Dashboard.Queries;

public record GetUserDashboardDetails : IRequest<DashboardDetails>
{
}

class GetUserDashboardDetailsValidator : AbstractValidator<GetUserDashboardDetails>
{
    public GetUserDashboardDetailsValidator()
    {
    }
}

class GetUserDashboardDetailsHandler : IRequestHandler<GetUserDashboardDetails, DashboardDetails>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper; 
    private readonly IIdentityService _identityService;
    private readonly IGraphService _graphService;



    public GetUserDashboardDetailsHandler(IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IGraphService graphService
        )
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _graphService = graphService;
    }

    public async Task<DashboardDetails> Handle(GetUserDashboardDetails request, CancellationToken cancellationToken)
    {

        /** Temporary Reference table for Dashboard Rank Labels and Score **/
        var dataDashboardRankLabels = new List<DashboardRankLabels>() {
            new DashboardRankLabels {
                Id = 1,
                Label = "Novice",
                Score = 100
            },
            new DashboardRankLabels {
                Id = 2,
                Label = "Enthusiast",
                Score = 500
            },
            new DashboardRankLabels {
                Id = 3,
                Label = "Learner",
                Score = 1000
            },
            new DashboardRankLabels {
                Id = 4,
                Label = "Advocate",
                Score = 2300
            },
            new DashboardRankLabels {
                Id = 5,
                Label = "Master",
                Score = 3500
            }
        };
        /** ------------------------------------ **/

        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        var user = await _graphService.GetUser(userEmail);

        var userChallengeRecordSummary = await _context.ChallengeRecordSummaries
           .Where(x => x.User.Email.ToLower() == userEmail.ToLower())
           .ProjectTo<LeaderboardResult>(_mapper.ConfigurationProvider)
           .FirstOrDefaultAsync();

        if (userChallengeRecordSummary == null)
        {
            userChallengeRecordSummary = new LeaderboardResult()
            {
                CurrentPoints = 0,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
            };
        }

        var dashboardRanks = new List<DashboardRank>();
        var minScore = dataDashboardRankLabels.Min(x => x.Score);
        var maxScore = dataDashboardRankLabels.Max(x => x.Score);

        for (int i = 0; i < dataDashboardRankLabels.Count; i++) {
            dashboardRanks.Add(
                new DashboardRank {
                    Label = dataDashboardRankLabels[i].Label,
                    Score = dataDashboardRankLabels[i].Score.ToString("#,##0"),
                    IsActive = (dataDashboardRankLabels[i].Score <= userChallengeRecordSummary.CurrentPoints)
                }
            );
        }

        var userDashboardDetails = new DashboardDetails()
        {
            UserName = user.FirstName,
            CurrentPoints = userChallengeRecordSummary.CurrentPoints,
            CurrentRankLabel = (userChallengeRecordSummary.CurrentPoints < minScore) ? "" : dataDashboardRankLabels
                .Last(x => x.Score <= userChallengeRecordSummary.CurrentPoints).Label,
            NextRankLabel = (userChallengeRecordSummary.CurrentPoints > maxScore) ? "" :dataDashboardRankLabels
                .First(x => x.Score > userChallengeRecordSummary.CurrentPoints).Label,
            RemainingPoints = ((userChallengeRecordSummary.CurrentPoints >= maxScore) ? 0 : (dataDashboardRankLabels
                .First(x => x.Score > userChallengeRecordSummary.CurrentPoints).Score) - userChallengeRecordSummary.CurrentPoints).ToString("#,##0"),
            MinScore = minScore,
            MaxScore = maxScore,
            DashboardRanks = dashboardRanks
        };

        return userDashboardDetails;
    }
}
