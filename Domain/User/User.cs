// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.Teams.Apps.Sustainability.Domain;

public class User: BaseAuditableEntity
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public virtual ICollection<UserRole>? UserRoles { get; set; }
    [NotMapped]
    public string? Initial { get; set; }
    [NotMapped]
    public bool HasPhoto { get; set; }
    [NotMapped]
    public string? Photo { get; set; }
    public virtual ICollection<ChallengeRecord>? ChallengeRecords { get; set; }
    
}
