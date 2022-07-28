// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Domain;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public interface IApplicationDbContext
{
    DbSet<Carousel> Carousels { get; }
    DbSet<ChallengeRecord> ChallengeRecords { get; }
    DbSet<ChallengeRecordSummary> ChallengeRecordSummaries { get; }
    DbSet<Challenge> Challenges { get; }
    DbSet<ChallengeInvite> ChallengeInvites { get; }
    DbSet<SiteConfig> SiteConfigs { get; }
    DbSet<Domain.User> Users { get; }
    DbSet<Role> Roles { get;}
    DbSet<UserRole> UserRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    int SaveChanges();
}
