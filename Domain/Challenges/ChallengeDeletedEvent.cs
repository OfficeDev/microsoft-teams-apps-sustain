// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class ChallengeDeletedEvent : BaseEvent
{
    public ChallengeDeletedEvent(Challenge item)
    {
        Item = item;
    }

    public Challenge Item { get; }
}
