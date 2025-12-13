using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Common.Extensions;
using Million.Application.Owners.Commands;
using Million.Application.Owners.DTOs;
using Million.Application.Owners.Queries;
using Million.Domain.Entities;

namespace Million.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todos los propietarios
    /// </summary>
    /// <returns>Lista de propietarios</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAllOwners()
    {
        var query = new GetAllOwnersQuery();
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
    /// Obtener un propietario por ID
    /// </summary>
    /// <param name="id">ID del propietario</param>
    /// <returns>Datos del propietario</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OwnerDto>> GetOwnerById(Guid id)
    {
        var query = new GetOwnerByIdQuery(id);
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
}
