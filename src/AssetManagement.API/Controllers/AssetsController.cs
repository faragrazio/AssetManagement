using AssetManagement.Application.Assets.Commands.CreateAsset;
using AssetManagement.Application.Assets.Commands.DeleteAsset;
using AssetManagement.Application.Assets.Commands.UpdateAsset;
using AssetManagement.Application.Assets.Queries.GetAllAssets;
using AssetManagement.Application.Assets.Queries.GetAssetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // tutti gli endpoint richiedono autenticazione JWT
public class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/assets
    // GET api/assets?category=Macchinario
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllAssetsQuery(category), cancellationToken);

        return Ok(result.Value);
    }

    // GET api/assets/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAssetByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    // POST api/assets
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        // 201 Created con header Location: api/assets/{id}
        return CreatedAtAction(nameof(GetById),
            new { id = result.Value }, new { id = result.Value });
    }

    // PUT api/assets/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAssetCommand command,
        CancellationToken cancellationToken)
    {
        // Assicura che l'ID nel path coincida con quello nel body
        if (id != command.Id)
            return BadRequest(new { error = "ID nel path e nel body non coincidono." });

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return NoContent(); // 204 — operazione riuscita senza corpo
    }

    // DELETE api/assets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteAssetCommand(id), cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return NoContent(); // 204
    }
}