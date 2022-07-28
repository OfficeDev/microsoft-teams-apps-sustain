// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Infrastructure;

const string AllowDevOriginPolicyName = "AllowDevTeamsAppsOrigin";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var keyVaultEndpoint = new Uri($"https://{configuration["KeyVaultName"]}.vault.azure.net/");
var defaultCredentials = new DefaultAzureCredential();

configuration.AddAzureKeyVault(keyVaultEndpoint, defaultCredentials,
    new AzureKeyVaultConfigurationOptions
    {
        ReloadInterval = TimeSpan.FromMinutes(5)
    });

services.AddApplicationServices();
services.AddInfrastructureServices(configuration);

services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));

services.AddMvc();
services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = 50000000; //50mb
    x.MultipartBodyLengthLimit = 50000000; //50mb
    x.MemoryBufferThreshold = 50000000; //50mb
    x.ValueCountLimit = 1024; //default 1024
});

var azureAdConfig = configuration.GetSection("AzureAd");
var graphConfig = configuration.GetSection("MSGraph");
var aadIdentity = azureAdConfig.Get<MicrosoftIdentityOptions>();
var validAudiences = azureAdConfig.GetValue<string>("Audiences").Split(' ');

services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme).Configure(options =>
{
    options.TokenValidationParameters.ValidAudiences = validAudiences;
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddMicrosoftGraph(graphConfig)
    .AddInMemoryTokenCaches();

services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

services.AddCors(options =>
    options.AddPolicy(
        name: AllowDevOriginPolicyName,
        builder => builder.WithOrigins("https://dev.teamsapps.local:3000")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
    )
);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    var scopes = azureAdConfig.GetValue<string>("Scopes").Split(" ");

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri($"{aadIdentity.Instance}{aadIdentity.TenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{aadIdentity.Instance}{aadIdentity.TenantId}/oauth2/v2.0/token"),
                Scopes = scopes.ToDictionary(scope => $"{aadIdentity.ClientId}/{scope}", scope => ""),
            }
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(AllowDevOriginPolicyName);

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(aadIdentity.ClientId);
    });

    app.UseDevSeedData();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

