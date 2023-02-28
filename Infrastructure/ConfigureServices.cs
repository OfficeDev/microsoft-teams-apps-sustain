// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Infrastructure.Services.Graph;
using PnP.Core.Services;
using PnP.Core.Services.Builder.Configuration;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useInMemoryDatabase)
                options.UseInMemoryDatabase("Sustainability");
            else
                options.UseSqlServer(configuration.GetValue<string>("necsus-sql-connection-string"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)).AddInterceptors(new AzureSqlConnectionTokenInjector());
        });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddTransient<ApplicationDbContextInitializer>();

        services.AddHttpContextAccessor();

        services.AddTransient<IGraphService, GraphService>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddSingleton<IDateTime, DateTimeService>();

        services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandlerService>();
        services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();


        if (useInMemoryDatabase)
        {
            services.AddSingleton<IBlobService, InMemoryBlobService>();
        }
        else
        {
            string blobStorageUri = configuration.GetSection("BlobStorage").GetSection("Uri").Value;
            string accountName = configuration.GetSection("BlobStorage").GetSection("AccountName").Value;
            string key = configuration.GetValue<string>("necsus-blob-key");
            string container = configuration.GetSection("BlobStorage").GetSection("Container").Value;

            services.AddSingleton<IBlobService>(
                x => new CloudBlobService(
                    new Uri(blobStorageUri),
                    accountName,
                    key,
                    container
                )
            );
        }

        services.Configure<PnPCoreOptions>(configuration.GetSection("PnPCore"));
        services.AddScoped<IAuthenticationProvider, SharePointAuthenticationProvider>();
        services.AddPnPCore();

        services.AddScoped<ISharePointService, SharePointService>();

        return services;
    }

    public static void UseDevSeedData(this IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        initializer.InitializeAsync().Wait();
        initializer.TrySeedAsync().Wait();
    }
}
