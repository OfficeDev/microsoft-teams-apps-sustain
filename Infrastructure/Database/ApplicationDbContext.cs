// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Domain;
using System.Reflection;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) 
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<Carousel> Carousels => Set<Carousel>();
    public DbSet<ChallengeRecord> ChallengeRecords => Set<ChallengeRecord>();
    public DbSet<ChallengeRecordSummary> ChallengeRecordSummaries => Set<ChallengeRecordSummary>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeInvite> ChallengeInvites => Set<ChallengeInvite>();
    public DbSet<SiteConfig> SiteConfigs => Set<SiteConfig>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
