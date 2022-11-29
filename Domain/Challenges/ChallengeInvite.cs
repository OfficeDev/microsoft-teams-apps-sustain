// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class ChallengeInvite: BaseAuditableEntity
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public virtual Challenge Challenge { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsAccepted { get; set; }
}
