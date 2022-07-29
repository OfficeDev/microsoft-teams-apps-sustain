// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using PnP.Core.Services;
using System.Net.Http.Headers;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

internal class SharePointAuthenticationProvider : IAuthenticationProvider
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly string _sharePointDomain;

    public SharePointAuthenticationProvider(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _tokenAcquisition = tokenAcquisition;

        _sharePointDomain = configuration.GetValue<string>("SharePointDomain");
    }

    public async Task AuthenticateRequestAsync(Uri resource, HttpRequestMessage request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        var token = await GetAccessTokenAsync(resource).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    public Task<string> GetAccessTokenAsync(Uri resource, string[] scopes)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (scopes == null) throw new ArgumentNullException(nameof(scopes));

        return _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
    }

    public Task<string> GetAccessTokenAsync(Uri resource)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        var scopes = new[] { $"https://{_sharePointDomain}/Sites.Search.All" };
        return GetAccessTokenAsync(resource, scopes);
    }
}
