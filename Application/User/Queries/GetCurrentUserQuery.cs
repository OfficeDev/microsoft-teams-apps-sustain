// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Microsoft.Teams.Apps.Sustainability.Application.User.Queries;

public record GetCurrentUserQuery: IRequest<UserSummaryResult>
{
}

class GetUserByIdQueryValidator: AbstractValidator<GetCurrentUserQuery>
{
    public GetUserByIdQueryValidator()
    {
    }
}

class GetUserByIdQueryHandler : IRequestHandler<GetCurrentUserQuery, UserSummaryResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(IApplicationDbContext dbContext, IIdentityService identityService, IMapper mapper)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<UserSummaryResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        string email = _identityService.CurrentUserEmail != null ? _identityService.CurrentUserEmail : "";

        if (!string.IsNullOrEmpty(email))
        {
            await InsertUserRoleIfNotExists(email, cancellationToken);
        }

        var userSummaryModel = _dbContext.Users
            .Include(x => x.UserRoles)
            .Where(x => x.Email.ToLower() == email.ToLower())
            .ProjectTo<UserSummaryResult>(_mapper.ConfigurationProvider)
            .FirstOrDefault();

        return userSummaryModel;
    }

    public async Task InsertUserRoleIfNotExists(string email, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.Include(x => x.UserRoles).FirstOrDefaultAsync(x => x.Email == email);

        // Null handlers
        if (user == null
            || (user != null && user.UserRoles == null)
            || (user != null && user.UserRoles != null && user.UserRoles.Count <= 0)
            )
        {
            var role = _dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == "user");

            if (user == null)
            {
                Domain.User newUser = new Domain.User()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = email,
                    Email = email,
                    Username = email
                };

                _dbContext.Users.Add(newUser);

                UserRole userRole = new UserRole()
                {
                    User = newUser,
                    Role = role
                };

                _dbContext.UserRoles.Add(userRole);
            }

            if (user != null && user.UserRoles == null || (user != null && user.UserRoles != null && user.UserRoles.Count <= 0))
            {
                UserRole userRole = new UserRole()
                {
                    User = user,
                    Role = role
                };

                _dbContext.UserRoles.Add(userRole);

            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
