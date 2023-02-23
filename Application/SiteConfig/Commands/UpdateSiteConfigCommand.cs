// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentValidation;
using MediatR;
using Microsoft.Teams.Apps.Sustainability.Domain;

namespace Microsoft.Teams.Apps.Sustainability.Application;
public record UpdateSiteConfigCommand : IRequest<int>
{
    public int ServiceType { get; set; }
    public string URI { get; set; } = "";
    public bool? IsEnabled { get; set; }
    public bool? IsNewsEnabled { get; set; }
    public bool? IsEventsEnabled { get; set; }
    public string? yammerGroupId { get; set; }
}


class UpdateSiteConfigCommandValidator : AbstractValidator<UpdateSiteConfigCommand>
{
    public UpdateSiteConfigCommandValidator()
    {
        RuleFor(v => v.URI)
            .MaximumLength(200)
            .NotEmpty();
    }
}

class UpdatesiteConfigCommandHandler : IRequestHandler<UpdateSiteConfigCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public UpdatesiteConfigCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(UpdateSiteConfigCommand request, CancellationToken cancellationToken)
    {
        string userEmail = _identityService.CurrentUserEmail;
        var date = DateTime.Now;
        var configEnabled = !string.IsNullOrEmpty(request.URI) ? true : false;

        SiteConfig dbModel = new SiteConfig()
        {
            Created = date,
            CreatedBy = userEmail,
            IsEnabled = configEnabled,
            IsEventsEnabled = request.IsEventsEnabled,
            IsNewsEnabled = request.IsNewsEnabled,
            LastModified = date,
            LastModifiedBy = userEmail,
            ServiceType = (SiteConfigServiceType) request.ServiceType,
            URI = request.URI,
            yammerGroupId = request.yammerGroupId
        };

        var serviceType = (SiteConfigServiceType)request.ServiceType;
        var existingRecord = _context.SiteConfigs.FirstOrDefault(x => x.ServiceType == serviceType);

        if (existingRecord == null)
        {
            _context.SiteConfigs.Add(dbModel);
        }
        else
        {
            existingRecord.LastModified = date;
            existingRecord.LastModifiedBy = userEmail;
            existingRecord.IsEnabled = configEnabled;
            existingRecord.IsEventsEnabled = request.IsEventsEnabled;
            existingRecord.IsNewsEnabled = request.IsNewsEnabled;
            existingRecord.URI = request.URI;
            existingRecord.yammerGroupId = request.yammerGroupId;
        }

        await _context.SaveChangesAsync(cancellationToken);


        return 1;
    }
}