// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.Teams.Apps.Sustainability.Domain;
using PnP.Core.Model.SharePoint;
using PnP.Core.Services;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class SharePointService : ISharePointService
{
    private static readonly Func<string, string> NewsQuery = (string siteUrl) =>
         $"Path:{siteUrl} AND IsDocument:True AND FileExtension:aspx AND PromotedState:2 AND (ModernAudienceAadObjectIds:{{User.Audiences}} OR NOT IsAudienceTargeted:true)";
    private static readonly List<string> NewsSelectProperties =
        new() { "Title", "Description", "Author", "Path", "PictureThumbnailURL", "ViewsRecent", "SiteLogo", "SiteTitle", "LastModifiedTime", "FirstPublishedDate" };
    
    private static readonly Func<string, string> EventsQuery = (string siteUrl) =>
        $"Path:{siteUrl} AND ContentTypeId:0x0102* AND EventDateOWSDATE>Today AND (ModernAudienceAadObjectIds:{{User.Audiences}} OR NOT IsAudienceTargeted:true)";
    private static readonly List<string> EventsSelectProperties =
        new() { "Title", "Description", "Author", "Path", "Location", "IsAllDayEvent", "EndDateOWSDATE", "EventDateOWSDATE", "ListId", "ListItemId" };

    private readonly IPnPContextFactory _factory;
    private readonly IAuthenticationProvider _auth;
    private readonly IApplicationDbContext _dbContext;

    public SharePointService(IPnPContextFactory factory, IAuthenticationProvider auth, IApplicationDbContext dbContext)
    {
        _factory = factory;
        _auth = auth;
        _dbContext = dbContext;
    }

    public async Task<List<SharePointNews>> GetRecentNews(int top, CancellationToken cancellationToken)
    {
        var spoConfig = _dbContext.SiteConfigs.FirstOrDefault(c => c.ServiceType == SiteConfigServiceType.SharePoint);

        if (spoConfig == null || !spoConfig.IsEnabled.GetValueOrDefault() || !spoConfig.IsNewsEnabled.GetValueOrDefault())
        {
            return new List<SharePointNews>();
        }

        var siteUrl = spoConfig.URI;

        using var context = await _factory.CreateAsync(new Uri(siteUrl), _auth);
        var query = new SearchOptions(NewsQuery(siteUrl))
        {
            SelectProperties = NewsSelectProperties,
            RowLimit = top,
            ClientType = "ContentSearchRegular"
        };

        var result = await context.Web.SearchAsync(query);

        var siteTitle = result.Rows.Select(r => r["SiteTitle"].ToString()).FirstOrDefault() ?? "";
        var seeAllUrl = $"{siteUrl}/_layouts/15/news.aspx?title={Uri.EscapeDataString(siteTitle + " News")}&audienceTargetingEnabled=true";

        return result.Rows.Select(r => new SharePointNews
        {
            Title = r["Title"]?.ToString(),
            Description = r["Description"]?.ToString(),
            Author = r["Author"]?.ToString(),
            Path = r["Path"]?.ToString(),
            PictureThumbnailURL = r["PictureThumbnailURL"]?.ToString(),
            ViewsRecent = Convert.ToInt32(r["ViewsRecent"]),
            SiteTitle = r["SiteTitle"]?.ToString(),
            SiteLogo = r["SiteLogo"]?.ToString() ?? $"{siteUrl}/_api/siteiconmanager/getsitelogo?type='1'",
            LastModifiedTime = Convert.ToDateTime(r["LastModifiedTime"]),
            FirstPublishedDate = Convert.ToDateTime(r["FirstPublishedDate"]),
            SeeAllUrl = seeAllUrl
        }).ToList();
    }

    public async Task<List<SharePointEvent>> GetUpcomingEvents(int top, CancellationToken cancellationToken)
    {
        var spoConfig = _dbContext.SiteConfigs.FirstOrDefault(c => c.ServiceType == SiteConfigServiceType.SharePoint);

        if (spoConfig == null || !spoConfig.IsEnabled.GetValueOrDefault() || !spoConfig.IsEventsEnabled.GetValueOrDefault())
        {
            return new List<SharePointEvent>();
        }

        var siteUrl = spoConfig.URI;

        using var context = await _factory.CreateAsync(new Uri(siteUrl), _auth);
        var query = new SearchOptions(EventsQuery(siteUrl))
        {
            SelectProperties = EventsSelectProperties,
            SortProperties = new()
            {
                new SortOption("EventDateOWSDATE", SortDirection.Ascending)
            },
            RowLimit = top,
            ClientType = "ContentSearchRegular"
        };
        
        var result = await context.Web.SearchAsync(query);        

        return result.Rows.Select(r => new SharePointEvent
        {
            Title = r["Title"]?.ToString(),
            Description = r["Description"]?.ToString(),
            Author = r["Author"]?.ToString(),
            Path = $"{siteUrl}/_layouts/15/Event.aspx?ListGuid={r["ListId"]?.ToString()}&ItemId={r["ListItemId"]?.ToString()}",
            Location = r["Location"]?.ToString(),
            EventDate = Convert.ToDateTime(r["EventDateOWSDATE"]),
            EndDate = Convert.ToDateTime(r["EndDateOWSDATE"]),
            IsAllDayEvent = Convert.ToBoolean(r["IsAllDayEvent"])
        }).ToList();
    }
}
