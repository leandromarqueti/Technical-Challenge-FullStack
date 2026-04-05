using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;
using TechnicalChallenge.Application.UseCases.Transactions.Commands.Delete;
using TechnicalChallenge.Application.UseCases.Transactions.Commands.Update;
using TechnicalChallenge.Application.UseCases.Transactions.Queries.GetById;
using TechnicalChallenge.Application.UseCases.Transactions.Queries.GetFiltered;
using TechnicalChallenge.Domain.Enums;

using Microsoft.AspNetCore.Authorization;

namespace TechnicalChallenge.API.Controllers;

[Authorize]
public class TransactionsController : BaseApiController
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] string? description,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? personId,
        [FromQuery] TransactionType? type,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFilteredTransactionsQuery
        {
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            CategoryId = categoryId,
            PersonId = personId,
            Type = type,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            UserId = UserId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionByIdQuery(id, UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        command.UserId = UserId;
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        command.UserId = UserId;
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTransactionCommand(id, UserId), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
