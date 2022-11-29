// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Teams.Apps.Sustainability.Domain;
namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task<bool> TrySeedAsync()
    {
        try
        {
            await SeedAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            return false;
        }
    }

    public async Task SeedAsync()
    {
        if (!_context.Challenges.Any())
        {
            _context.Challenges.Add(new Challenge
            {
                Title = "Turn off computer each day",
                Description = "Turn off your laptop at the end of each day before leaving the office in order to save energy",
                Recurrence = ChallengeRecurrence.Daily,
                ActiveUntil = DateTime.Today + TimeSpan.FromDays(30),
                IsActive = true
            });

var roles = new List<Role>()
            {
                new Role()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = "Initializer",
                    Name = "Admin",
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = "Initializer"
                },
                new Role()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = "Initializer",
                    Name = "User",
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = "Initializer"
                }
            };

            _context.Roles.AddRange(roles);

            var siteConfigs = new List<SiteConfig>()
            {
                new SiteConfig()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = "Initializer",
                    IsEnabled = false,
                    IsEventsEnabled = false,
                    IsNewsEnabled = false,
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = "Initializer",
                    ServiceType = SiteConfigServiceType.SharePoint,
                    URI = ""
                },
                new SiteConfig()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = "Initializer",
                    IsEnabled = false,
                    IsEventsEnabled = false,
                    IsNewsEnabled = false,
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = "Initializer",
                    ServiceType = SiteConfigServiceType.Yammer,
                    URI = ""
                }
            };

            _context.SiteConfigs.AddRange(siteConfigs);

            await _context.SaveChangesAsync();
        }
    }
}
