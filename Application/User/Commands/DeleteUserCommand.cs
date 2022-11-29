// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;
public record class DeleteUserCommand : IRequest<int>
{
    public string Email { get; set; } = "";
}

class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(v => v.Email)
            .MinimumLength(2)
            .NotEmpty();
    }
}

class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public DeleteUserCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        int result = 0;

        string currentUserEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";

        var userObject = _context.Users.Include(x => x.UserRoles).FirstOrDefault(x => x.Email == request.Email);

        if (userObject == null)
        {
            throw new NotFoundException(nameof(User), request.Email);
        }

        if (userObject != null)
        {
            _context.UserRoles.RemoveRange(userObject.UserRoles);
            _context.Users.Remove(userObject);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
