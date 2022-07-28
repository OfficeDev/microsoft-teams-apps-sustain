// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application;

public interface IDateTime
{
    DateTime Today { get; }
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
