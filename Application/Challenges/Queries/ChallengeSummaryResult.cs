// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;
using Microsoft.Teams.Apps.Sustainability.Domain;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class ChallengeSummaryResult : IMapFrom<Challenge>
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public int Points { get; set; }
    public string? Thumbnail { get; set; }
    public string Description { get; set; } = "";
    public DateTimeOffset ActiveUntil { get; set; }
    public ChallengeRecurrence Recurrence { get; set; }
    public string? FocusArea { get; set; }
    public string? AdditionalResources { get; set; }
    public bool IsActive { get; set; } = true;
    public bool? IsPinned { get; set; }
    public IEnumerable<ChallengeRecordSummaryResult>? ChallengeRecords { get; set; }
    public IEnumerable<UserWithPhotoModel>? RelatedUsers { get; set; }
    public int FinalStatus { get; set; }
}

public class ChallengeSummaryResultStatus : IMapFrom<ChallengeRecord>
{
    public ChallengeRecordStatus Status { get; set; }
}

public class UserAcceptedCount
{
    public int Id { get; set; }

    public string ChallengePoint { get; set; } = "";

    public int TotalCount { get; set; }

    public ChallengeRecordStatus Status { get; set; }
}

