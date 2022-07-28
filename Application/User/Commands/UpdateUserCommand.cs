// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record class UpdateUserCommand : IRequest<int>
{
    public string Email { get; set; } = "";
    public string Role { get; set; }
}

class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(v => v.Role)
            .MinimumLength(4)
            .NotEmpty();
    }
}

class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        int result = 0;

        var role = _context.Roles.FirstOrDefault(x => x.Name.ToLower() == request.Role.ToLower());

        string currentUserEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        var userObject = _context.Users.Include(x => x.UserRoles).FirstOrDefault(x => x.Email == request.Email);

        if (userObject == null)
        {
            throw new NotFoundException(nameof(User), request.Email);
        }

        if (userObject != null)
        {
            userObject.UserRoles = null;

            var userRoleObject = new UserRole()
            {
                Role = role,
                User = userObject
            };

            _context.UserRoles.Add(userRoleObject);
            userObject.UserRoles = new List<UserRole>() { userRoleObject };

            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
