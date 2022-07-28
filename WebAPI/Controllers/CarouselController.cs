// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI;

[Authorize(Policy = "RequiredRoleUser,Admin")]
public class CarouselController : WebApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<CarouselSummaryResult>>> Get(int pageNumber = 1, int pageSize = 10)
    {
        return await Mediator.Send(new GetCarouselQuery() { PageNumber = pageNumber, PageSize = pageSize });
    }

    [HttpPost]
    public async Task<int> Post()
    {
        int result = 0;

        var requestList = new List<AddCarouselCommand>();

        var count = int.Parse(Request.Form.FirstOrDefault(x => x.Key == "ItemsCount").Value);

        for (int i = 0; i < count; i++)
        {
            var request = new AddCarouselCommand();

            var link = Request.Form.FirstOrDefault(x => x.Key == $"Link_{i}").Value;
            var title = Request.Form.FirstOrDefault(x => x.Key == $"Title_{i}").Value;
            var description = Request.Form.FirstOrDefault(x => x.Key == $"Description_{i}").Value;
            var isActive = Request.Form.FirstOrDefault(x => x.Key == $"IsActive_{i}").Value;                
            request.Link = link;
            request.Title = title;
            request.Description = description;
            request.IsActive = bool.Parse(isActive);

            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.Where(x => x.Name == $"ThumbnailFile_{i}").FirstOrDefault();
                if (file != null && !string.IsNullOrEmpty(request.Link))
                {
                    if (file.Length <= 1000000 && (file.ContentType.ToLower() == "image/png" || file.ContentType.ToLower() == "image/jpg" || file.ContentType.ToLower() == "image/jpeg"))
                    {
                        using (var stream = new MemoryStream())
                        {
                            stream.Position = 0;
                            file.CopyTo(stream);
                            request.Photo = stream;
                            request.PhotoFileName = Request.Form.FirstOrDefault(x => x.Key == $"ThumbnailFilename_{i}").Value;

                            result = await Mediator.Send(request);
                            if (result == 0)
                                return 0;
                        }
                    }
                }
            }
        }

        return 1;
    }

    [HttpPut]
    public async Task<int> Put()
    {
        int result = 0;

        var requestList = new List<EditCarouselCommand>();
        
        var count = int.Parse(Request.Form.FirstOrDefault(x => x.Key == "ItemsCount").Value);

        for (int i= 0; i < count; i++)
        {
            var request = new EditCarouselCommand();

            var id = Request.Form.FirstOrDefault(x => x.Key == $"Id_{i}").Value;
            var link = Request.Form.FirstOrDefault(x => x.Key == $"Link_{i}").Value;
            var title = Request.Form.FirstOrDefault(x => x.Key == $"Title_{i}").Value;
            var description = Request.Form.FirstOrDefault(x => x.Key == $"Description_{i}").Value;
            var isActive = Request.Form.FirstOrDefault(x => x.Key == $"IsActive_{i}").Value;
            var thumbnail = Request.Form.FirstOrDefault(x => x.Key == $"Thumbnail_{i}").Value;
            request.Id = int.Parse(id);
            request.Link = link;
            request.Title = title;
            request.Description = description;
            request.Thumbnail = thumbnail;
            request.IsActive = bool.Parse(isActive);

            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                //update photo
                var file = Request.Form.Files.Where(x => x.Name == $"ThumbnailFile_{i}").FirstOrDefault();
                if (file != null && !string.IsNullOrEmpty(request.Link))
                {
                    if (file.Length <= 1000000 && (file.ContentType.ToLower() == "image/png" || file.ContentType.ToLower() == "image/jpg" || file.ContentType.ToLower() == "image/jpeg"))
                    {
                        using (var stream = new MemoryStream())
                        {
                            stream.Position = 0;
                            file.CopyTo(stream);
                            request.Photo = stream;
                            request.PhotoFileName = Request.Form.FirstOrDefault(x => x.Key == $"ThumbnailFilename_{i}").Value;

                            result = await Mediator.Send(request);
                            if (result == 0)
                                return 0;
                        }
                    }
                }
            }

            if(string.IsNullOrEmpty(request.PhotoFileName) && !string.IsNullOrEmpty(request.Link))
            {
                //Update texts
                result = await Mediator.Send(request);
                if (result == 0)
                    return 0;
            }
        }

        return 1;
    }

    [HttpDelete]
    public async Task<int> Delete(int [] ids)
    {
        var result = 0;
        foreach (int id in ids)
        {
            result = await Mediator.Send(new DeleteCarouselCommand() { Id = id });

            if (result == 0)
                return 0;
        }

        return 1;
    }
}
