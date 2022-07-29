// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? CurrentUserEmail
        {
            // preferred_username
            //_contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "preferred_username").
            get 
            {
                string email = string.Empty;
                var claimsEmailValue = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "preferred_username");
                var claimsEmailValueAlt = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"); 
                
                if (claimsEmailValue != null)
                {
                    email = claimsEmailValue.Value;
                }

                if (claimsEmailValueAlt != null)
                {
                    email = claimsEmailValueAlt.Value;
                }

            return email; 
            
            }
        }
    }
