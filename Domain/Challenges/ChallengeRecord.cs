// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class ChallengeRecord: BaseAuditableEntity
{
    public virtual User User { get; set; }
    public virtual Challenge Challenge { get; set; }
    public int ChallengePoint { get; set; }
    public virtual ChallengeRecordStatus Status { get; set; }
}
