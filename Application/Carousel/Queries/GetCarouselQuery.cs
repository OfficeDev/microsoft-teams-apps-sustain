// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;

namespace Microsoft.Teams.Apps.Sustainability.Application;
public record GetCarouselQuery : IRequest<PaginatedList<CarouselSummaryResult>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}


class GetCarouselQueryValidator : AbstractValidator<SiteConfigQuery>
{
    public GetCarouselQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

class GetCarouselQueryHandler : IRequestHandler<GetCarouselQuery, PaginatedList<CarouselSummaryResult>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    public GetCarouselQueryHandler(IApplicationDbContext dbContext, IMapper mapper, IBlobService blobService)
    {
        _dbContext = dbContext;
        _mapper=mapper;
        _blobService=blobService;
    }

    public async Task<PaginatedList<CarouselSummaryResult>> Handle(GetCarouselQuery request, CancellationToken cancellationToken)
    {
        var result = await _dbContext
            .Carousels
            .Where(x => x.IsActive)
            .ProjectTo<CarouselSummaryResult>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        foreach(var item in result.Items)
        {
            var thumbnailLink = item.Thumbnail;
            if (!string.IsNullOrEmpty(thumbnailLink))
            {
                string newLink = _blobService.GetSasLink(thumbnailLink);
                item.Thumbnail = newLink;

                var splitThumbnail = thumbnailLink.Split('/');
                item.Filename = splitThumbnail[splitThumbnail.Length-1];
            }
        }

        return result;
    }
}

