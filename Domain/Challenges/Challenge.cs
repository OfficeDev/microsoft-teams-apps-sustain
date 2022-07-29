// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class Challenge : BaseAuditableEntity
{
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
    public virtual ICollection<ChallengeRecord>? ChallengeRecords { get; set; }
}
