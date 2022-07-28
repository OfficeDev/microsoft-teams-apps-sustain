// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class RequiredRole: IAuthorizationRequirement
{
    public string Roles { get; set; }
    /// <summary>
    /// Specifies a specific role
    /// </summary>
    /// <param name="roles">Comma separated role names</param>
    public RequiredRole(string roles)
    {
        Roles=roles;    
    }

    /// <summary>
    /// Returns required roles.
    /// </summary>
    /// <returns></returns>
    public List<string> GetRequiredroles()
    {

        List<string> result = new List<string>();

        if (!string.IsNullOrEmpty(Roles))
        {
            // Remove spaces
            Roles = Roles.Replace(" ", "");
            // Splits by comma
            result = Roles.Split(",").ToList();
        }

        return result;
    }
}
