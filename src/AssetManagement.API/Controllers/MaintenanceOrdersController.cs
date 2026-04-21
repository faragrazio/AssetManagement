using AssetManagement.Application.MaintenanceOrders.Commands.CreateOrder;
using AssetManagement.Application.MaintenanceOrders.Commands.UpdateOrderStatus;
using AssetManagement.Application.MaintenanceOrders.Queries.GetAllOrders;
using AssetManagement.Application.MaintenanceOrders.Queries.GetOrdersByAsset;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MaintenanceOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/maintenanceorders
    // GET api/maintenanceorders?status=Pending
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllOrdersQuery(status), cancellationToken);

        return Ok(result.Value);
    }

    // GET api/maintenanceorders/asset/5
    [HttpGet("asset/{assetId:int}")]
    public async Task<IActionResult> GetByAsset(
        int assetId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOrdersByAssetQuery(assetId), cancellationToken);

        return Ok(result.Value);
    }

    // POST api/maintenanceorders
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetByAsset),
            new { assetId = command.AssetId }, new { id = result.Value });
    }

    // PATCH api/maintenanceorders/5/status
    [HttpPatch("{orderId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int orderId,
        [FromBody] UpdateOrderStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (orderId != command.OrderId)
            return BadRequest(new { error = "ID nel path e nel body non coincidono." });

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}