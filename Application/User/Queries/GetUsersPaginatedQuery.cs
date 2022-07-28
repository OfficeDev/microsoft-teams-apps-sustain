// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record GetUsersPaginatedQuery : IRequest<PaginatedList<UserSummaryResult>>
{
    public string? Role { get; set; } = null;
    public string? Search { get; set; } = null;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

class GetUsersPaginatedQueryValidator : AbstractValidator<GetUsersPaginatedQuery>
{
    public GetUsersPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedList<UserSummaryResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetUsersPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;       
        _mapper = mapper;
    }

    public async Task<PaginatedList<UserSummaryResult>> Handle(GetUsersPaginatedQuery request, CancellationToken cancellationToken)
    {
        var usersQuery = _context.Users
            .Include("UserRoles")
            .Include("UserRoles.Role")
            .OrderBy(x => x.Email)
            .Where(x => true);

        if (!string.IsNullOrEmpty(request.Role))
        {
            usersQuery = usersQuery.Where(x => x.UserRoles.Any(r => r.Role.Name.ToLower() == request.Role));
        }

        if (!string.IsNullOrEmpty(request.Search))
        {
            var keyword = request.Search.ToLower();

            usersQuery = usersQuery.Where(
                x => x.Username.ToLower().Contains(keyword)
                || x.Email.ToLower().Contains(keyword)
            );
        }

        var userResult = await usersQuery
            .ProjectTo<UserSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return userResult;
    }

}
