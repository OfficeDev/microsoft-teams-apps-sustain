// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class DateTimeService : IDateTime
{
    public DateTime Today => DateTime.Today;
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
