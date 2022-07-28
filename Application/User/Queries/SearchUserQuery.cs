// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public record SearchUserQuery : IRequest<List<UserWithPhotoModel>>
{
    public string Query { get; set; } = "";
}

class SearchUserQueryValidator : AbstractValidator<SearchUserQuery>
{
    public SearchUserQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotNull().WithMessage("Search query cannot be null.");

    }
}

class SearchUserQueryHandler : IRequestHandler<SearchUserQuery, List<UserWithPhotoModel>>
{
    private readonly IGraphService _graph;
    public SearchUserQueryHandler(IGraphService graph)
    {
        _graph = graph;
    }   

    public async Task<List<UserWithPhotoModel>> Handle(SearchUserQuery request, CancellationToken cancellationToken)
    {
        List<UserWithPhotoModel> users = new List<UserWithPhotoModel>();

        if (!string.IsNullOrEmpty(request.Query))
        {
            users = await _graph.GetUsersStartWith(request.Query);
        }
        
        return users;
    }

}
