// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class ChallengeCreatedEvent : BaseEvent
{
    public ChallengeCreatedEvent(Challenge item)
    {
        Item = item;
    }

    public Challenge Item { get; }
}
