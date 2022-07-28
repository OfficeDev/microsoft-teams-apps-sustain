// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class RequiredRoleAttribute: AuthorizeAttribute
{
    const string POLICY_PREFIX = "RequiredRole";
    public RequiredRoleAttribute(string roles) => Roles = roles;

    public string Role
    {
        get
        {
            return Role;
        }
        set
        {
            Policy = $"{POLICY_PREFIX}{value}";
        }
    }
}
