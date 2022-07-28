// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record DeleteChallengeCommand(int Id) : IRequest<int>;

class DeleteTodoItemCommandHandler : IRequestHandler<DeleteChallengeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(DeleteChallengeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Challenges
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Challenge), request.Id);
        }


        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return 1;
    }
}
