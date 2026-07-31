using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/sales")]
public sealed class SalesController(ISaleService saleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SaleResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SaleResponse>>> GetAll(
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await saleService.GetAllAsync(take, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SaleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        return Ok(await saleService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<SaleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SaleResponse>> Create(
        CreateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var sale = await saleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }
}
