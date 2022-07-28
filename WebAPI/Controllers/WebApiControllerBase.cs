// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Teams.Apps.Sustainability.WebAPI;

[ApiController]
[Route("api/[controller]")]
public abstract class WebApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
