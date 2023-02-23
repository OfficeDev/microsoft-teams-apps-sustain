// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record class CreateUserCommand: IRequest<int>
{
    public List<string> Emails { get; set; } = new List<string>() { };
    public string Role { get; set; }
}

class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.Role)
            .MinimumLength(4)
            .NotEmpty();
    }
}

class CreateUserCommandHandler: IRequestHandler<CreateUserCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IGraphService _graphService;

    public CreateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService, IGraphService graphService)
    {
        _context = context;
        _identityService = identityService;
        _graphService = graphService;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        int result = 0;

        var role = _context.Roles.FirstOrDefault(x => x.Name.ToLower() == request.Role.ToLower());
        string groupID = "";
        groupID = _context.SiteConfigs.FirstOrDefault(x => x.ServiceType == SiteConfigServiceType.Yammer).yammerGroupId;

        string currentUserEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        List<Domain.User> userModels = new List<Domain.User>();
        List<UserRole> userRoleModels = new List<UserRole>();

        // duplicate validation
        var hasDuplicate = _context.Users.Any(x => request.Emails.Contains(x.Email));

        if (hasDuplicate)
        {
            throw new UserDuplicateException(nameof(User), "One of the email addresses has duplicates.");
        }
        List<string> userEmailCol = new List<string>();

        foreach (var email in request.Emails)
        {
            var userModel = new Domain.User()
            {
                Username = email,
                Email = email,
                Created = DateTime.UtcNow,
                CreatedBy = currentUserEmail,
            };

            var userRoleModel = new UserRole()
            {
                Role = role,
                User = userModel
            };

            userModels.Add(userModel);
            userRoleModels.Add(userRoleModel);
            var user = await _graphService.GetUser(email);
            userEmailCol.Add($"https://graph.microsoft.com/v1.0/directoryObjects/{user.Id}");
        }

        await _graphService.AddMemberToYammerGroup(userEmailCol, groupID);
        await _context.Users.AddRangeAsync(userModels);
        await _context.UserRoles.AddRangeAsync(userRoleModels);
        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}