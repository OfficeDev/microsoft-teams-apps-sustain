// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Application.Leaderboard.Queries
{
    public record GetLeaderboardQuery : IRequest<PaginatedList<LeaderboardResult>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool FilteredByOrg { get; init; } = false;
    }

    class GetLeaderboardQueryValidator : AbstractValidator<GetLeaderboardQuery>
    {
        public GetLeaderboardQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
        }
    }
    class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, PaginatedList<LeaderboardResult>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGraphService _graphService;

        public GetLeaderboardQueryHandler(IApplicationDbContext context, IMapper mapper, IGraphService graphService)
        {
            _context = context;
            _mapper = mapper;
            _graphService = graphService;
        }

        public async Task<PaginatedList<LeaderboardResult>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var relatedPeople = new List<UserWithPhotoModel>();
            if (!request.FilteredByOrg) {
                relatedPeople = await _graphService.GetRelatedPeople();

                var relatedResults = await _context.ChallengeRecordSummaries
                   .Where(x => relatedPeople.Select(y => y.EmailAddress).Contains(x.User.Email))
                   .OrderByDescending(x => x.CurrentPoints)
                   .ProjectTo<LeaderboardResult>(_mapper.ConfigurationProvider)
                   .PaginatedListAsync(request.PageNumber, request.PageSize);

                foreach(var item in relatedResults.Items)
                {
                    var user = relatedPeople.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && item.User.Email.ToLower() == x.EmailAddress.ToLower()).FirstOrDefault();

                    item.User.HasPhoto = user.HasPhoto;
                    item.User.Photo = user.Photo;
                    item.User.Initial = user.Initial;
                }

                return relatedResults;
            }
            else
            {
                var orgUser = await _context.ChallengeRecordSummaries
                    .OrderByDescending(x => x.CurrentPoints)
                    .ProjectTo<LeaderboardResult>(_mapper.ConfigurationProvider)
                    .PaginatedListAsync(request.PageNumber, request.PageSize);

                foreach(var item in orgUser.Items)
                {
                    var user = await _graphService.GetUser(item.User.Email);

                    item.User.HasPhoto = user.HasPhoto;
                    item.User.Photo = user.Photo;
                    if (!string.IsNullOrEmpty(user.Initial)) 
                    {
                        item.User.Initial = user.Initial;
                    }
                    else
                    {
                        var splittedName = item.User.Username.Split(' ');
                        try
                        {
                            item.User.Initial = $"{splittedName[0].Substring(0, 1)}{splittedName[splittedName.Length-1].Substring(0, 1)}";
                        }
                        catch
                        {
                            item.User.Initial = item.User.Username.Substring(0, 2);
                        }
                        
                    }

                }

                return orgUser;
            }
        }
    }
}
