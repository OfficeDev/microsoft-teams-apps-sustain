// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetUsersToInviteQuery : IRequest<List<UserWithPhotoModel>>
{

}

class GetUsersToInviteQueryValidator : AbstractValidator<GetUsersToInviteQuery>
{

}

class GetUsersToInviteQueryHandler : IRequestHandler<GetUsersToInviteQuery, List<UserWithPhotoModel>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IGraphService _graphService;

    public GetUsersToInviteQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IGraphService graphService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _graphService = graphService;
    }

    public async Task<List<UserWithPhotoModel>> Handle(GetUsersToInviteQuery request, CancellationToken cancellationToken)
    {
        string userEmail = !string.IsNullOrEmpty(_identityService.CurrentUserEmail) ? _identityService.CurrentUserEmail : "";
        var result = await _graphService.GetUsers(userEmail);
        return result;
    }
}
