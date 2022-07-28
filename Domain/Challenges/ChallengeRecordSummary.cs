// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class ChallengeRecordSummary: BaseAuditableEntity
{
    public virtual User User { get; set; }
    public int CurrentPoints { get; set; }
}
