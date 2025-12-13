using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Common.Extensions;
using Million.Application.PropertyTraces.Commands;
using Million.Application.PropertyTraces.Queries;
using Million.Application.Properties.DTOs;
using Million.Domain.Entities;

namespace Million.Api.Controllers;

[ApiController]
[Route("api/properties/{propertyId:guid}/traces")]
public class PropertyTracesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyTracesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener el historial de una propiedad
    /// </summary>
    /// <param name="propertyId">ID de la propiedad</param>
    /// <returns>Lista del historial de la propiedad</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PropertyTraceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PropertyTraceDto>>> GetPropertyTraces(Guid propertyId)
    {
        var query = new GetPropertyTracesQuery(propertyId);
        var result = await _mediator.Send(query);
        
        if (result.IsFailed)
        {
            var statusCode = result.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = result.GetFirstErrorMessage(),
                statusCode = statusCode
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Value,
            statusCode = 200
        });
    }

    /// <summary>
    /// Agregar un registro al historial de una propiedad
    /// </summary>
    /// <param name="propertyId">ID de la propiedad</param>
    /// <param name="request">Datos del registro</param>
    /// <returns>ID del registro creado</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddPropertyTrace(Guid propertyId, [FromBody] AddPropertyTraceRequest request)
    {
        var command = new AddPropertyTraceCommand(
            propertyId, 
            request.DateSale, 
            request.Name, 
            request.Value, 
            request.Tax);
        
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            var statusCode = result.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = result.GetFirstErrorMessage(),
                statusCode = statusCode
            });
        }

        return CreatedAtAction(nameof(GetPropertyTraces), new { propertyId }, new
        {
            success = true,
            data = result.Value,
            statusCode = 201
        });
    }
}

public class AddPropertyTraceRequest
{
    public DateTime DateSale { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
}
