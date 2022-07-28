// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application.Leaderboard.Queries
{
    public class LeaderboardResult : IMapFrom<ChallengeRecordSummary>
    {
        public int Id { get; set; }
        public Domain.User? User { get; set; }
        public int CurrentPoints { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
