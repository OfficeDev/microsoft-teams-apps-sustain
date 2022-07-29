// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class UserRoleSummaryResult : IMapFrom<UserRole>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public RoleSummaryResult Role { get; set; }
}
