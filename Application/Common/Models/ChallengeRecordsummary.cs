// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

public class ChallengeRecordsummary
{
    public int Id { get; set; }
    public UserWithPhotoModel? User { get; set; }
    public int CurrentPoints { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateModified { get; set; }
}
