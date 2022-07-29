// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class SharePointEvent : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Path { get; set; }
    public string? Location { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDayEvent { get; set; }
}
