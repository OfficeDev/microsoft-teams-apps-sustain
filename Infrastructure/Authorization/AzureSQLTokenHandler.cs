// Copyright (c) Microsoft. All Rights Reserved.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;
public class AzureSqlConnectionTokenInjector : DbConnectionInterceptor
{
    private DefaultAzureCredential _tokenProvider;

    public AzureSqlConnectionTokenInjector()
    {
        _tokenProvider = new DefaultAzureCredential();
    }

    protected virtual async Task EnsureAccessToken(DbConnection connection)
    {
        try
        {
            if (connection is SqlConnection sqlConnection
                && connection.ConnectionString.ToUpper().Contains("DATABASE.WINDOWS.NET")
                && string.IsNullOrWhiteSpace(sqlConnection.AccessToken))
                sqlConnection.AccessToken = (await _tokenProvider.GetTokenAsync(new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/" }))).Token;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        EnsureAccessToken(connection).Wait();

        return base.ConnectionOpening(connection, eventData, result);
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
    {
        await EnsureAccessToken(connection);

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}