// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetUserByEmailQuery : IRequest<UserSummaryResult>
{
    public string Email { get; set; } = "";
}

class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
    }
}

class GetUserByIdQueryHandler : IRequestHandler<GetUserByEmailQuery, UserSummaryResult>
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

    public async Task<UserSummaryResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userSummaryModel = _dbContext.Users
            .Include(x => x.UserRoles)
            .Where(x => x.Email.ToLower() == request.Email.ToLower())
            .ProjectTo<UserSummaryResult>(_mapper.ConfigurationProvider)
            .FirstOrDefault();

        return userSummaryModel;
    }
}