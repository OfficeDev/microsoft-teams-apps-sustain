// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;
namespace Microsoft.Teams.Apps.Sustainability.Application.Challenges.Queries;

public class ChallengeRecordSummaryResult : IMapFrom<ChallengeRecord>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ChallengeId { get; set; }
    public int ChallengePoint { get; set; }
    public ChallengeRecordStatus Status { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool CompleteYesterday { get; set; }
    public bool CompleteToday { get; set; }
}
