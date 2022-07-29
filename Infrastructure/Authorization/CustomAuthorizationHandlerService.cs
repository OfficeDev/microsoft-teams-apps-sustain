// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class CustomAuthorizationHandlerService : AuthorizationHandler<RequiredRole>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    public CustomAuthorizationHandlerService(IApplicationDbContext context, IIdentityService identityService)
    {
        _dbContext = context;
        _identityService=identityService;       
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiredRole requirement)
    {
        string email = _identityService.CurrentUserEmail != null ? _identityService.CurrentUserEmail : "";
        
        var user = GetUserObject(email);

        // validation for non-existing users
        if (
            user == null // no user record
            || (user != null && user.UserRoles == null) // has record but null roles
            || (user != null && user.UserRoles != null && user.UserRoles.Count <= 0) 
        )
        {
            // get required roles in lowercase
            var lowercaseRoles = requirement.GetRequiredroles().Select(x => x.ToLower()).ToList();

            // if no admin role, set succeed
            if (lowercaseRoles.Contains("user"))
            {
                context.Succeed(requirement);
            }
        }
        else
        {   
        // validation for existing users

            var userRolesStrArr = user.UserRoles.Select(x => x.Role.Name).ToList();

            bool isAuthorized = IsAuthorized(userRolesStrArr, requirement);

            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
        }

        

        return Task.CompletedTask;
    }

    private User GetUserObject(string email)
    {
        var user = _dbContext.Users
            .Include("UserRoles")
            .Include("UserRoles.Role")
            .FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

        return user;
    }

    private bool IsAuthorized(List<string> userRoles, RequiredRole requiredRole)
    {
        bool result = false;

        if (requiredRole != null)
        {
            userRoles.ForEach(x =>
            {
                if (requiredRole.GetRequiredroles().Contains(x))
                {
                    result = true;
                }
            });
        }

        return result;
    }
}

