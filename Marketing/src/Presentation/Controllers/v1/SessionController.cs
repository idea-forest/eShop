using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using YourBrand.Marketing.Application.Analytics;
using YourBrand.Marketing.Application.Analytics.Commands;
using YourBrand.Marketing.Domain.Enums;

namespace YourBrand.Marketing.Presentation.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<string> InitSession([FromHeader(Name = "X-Client-Id")] string clientId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new InitSessionCommand(clientId), cancellationToken);
    }

    [HttpPost("Coordinates")]
    public async Task RegisterCoordinates([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, [FromBody] Domain.ValueObjects.Coordinates coordinates, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RegisterGeoLocation(clientId, sessionId, coordinates), cancellationToken);
    }
}
