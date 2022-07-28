// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class UserRole: BaseAuditableEntity
{
    public virtual Role Role { get; set; }
    public virtual User User { get; set; }
}
