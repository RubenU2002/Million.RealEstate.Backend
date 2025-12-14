using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Common.Extensions;
using Million.Application.Common.Models;
using Million.Application.Properties.Commands;
using Million.Application.PropertyImages.Commands;
using Million.Application.Properties.DTOs;
using Million.Application.Properties.Queries;
using Million.Domain.Entities;

namespace Million.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Buscar propiedades con filtros y paginación
    /// </summary>
    /// <param name="name">Filtro por nombre</param>
    /// <param name="address">Filtro por dirección</param>
    /// <param name="minPrice">Precio mínimo</param>
    /// <param name="maxPrice">Precio máximo</param>
    /// <param name="minYear">Año mínimo de construcción</param>
    /// <param name="maxYear">Año máximo de construcción</param>
    /// <param name="pageNumber">Número de página (inicia en 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <returns>Lista paginada de propiedades</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<PropertyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<PropertyDto>>> SearchProperties(
        [FromQuery] string? name = null,
        [FromQuery] string? address = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? minYear = null,
        [FromQuery] int? maxYear = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SearchPropertiesQuery(
            name, 
            address, 
            minPrice, 
            maxPrice, 
            pageNumber, 
            pageSize);
        
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
            data = result.Value.Items,
            totalCount = result.Value.TotalCount,
            pageNumber = result.Value.PageNumber,
            pageSize = result.Value.PageSize,
            hasNextPage = result.Value.HasNextPage,
            hasPreviousPage = result.Value.HasPreviousPage,
            statusCode = 200
        });
    }

    /// <summary>
    /// Obtener una propiedad por ID
    /// </summary>
    /// <param name="id">ID de la propiedad</param>
    /// <returns>Detalles de la propiedad</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyDto>> GetPropertyById(Guid id)
    {
        var query = new GetPropertyByIdQuery(id);
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
    /// Obtener todas las propiedades de un propietario específico
    /// </summary>
    /// <param name="ownerId">ID del propietario</param>
    /// <returns>Lista de propiedades del propietario</returns>
    [HttpGet("owner/{ownerId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PropertyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PropertyDto>>> GetPropertiesByOwnerId(Guid ownerId)
    {
        var query = new GetPropertiesByOwnerIdQuery(ownerId);
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
    /// Crear una nueva propiedad
    /// </summary>
    /// <param name="command">Datos de la propiedad</param>
    /// <returns>ID de la propiedad creada</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Guid>> CreateProperty([FromBody] CreatePropertyCommand command)
    {
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

        return CreatedAtAction(nameof(GetPropertyById), new { id = result.Value }, new
        {
            success = true,
            data = result.Value,
            statusCode = 201
        });
    }

    /// <summary>
    /// Actualizar una propiedad existente
    /// </summary>
    /// <param name="id">ID de la propiedad</param>
    /// <param name="request">Datos actualizados de la propiedad</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var command = new UpdatePropertyCommand(
            id, 
            request.Name, 
            request.Description, 
            request.Address, 
            request.Price, 
            request.Year);
        
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

        return Ok(new
        {
            success = true,
            message = "Property updated successfully",
            statusCode = 200
        });
    }

    /// <summary>
    /// Eliminar una propiedad existente
    /// </summary>
    /// <param name="id">ID de la propiedad</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProperty(Guid id)
    {
        var command = new DeletePropertyCommand(id);
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

        return Ok(new
        {
            success = true,
            message = "Property deleted successfully",
            statusCode = 200
        });
    }

    /// <summary>
    /// Cambiar el precio de una propiedad
    /// </summary>
    /// <param name="id">ID de la propiedad</param>
    /// <param name="request">Nuevo precio</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPatch("{id:guid}/price")]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePropertyPrice(Guid id, [FromBody] UpdatePropertyPriceRequest request)
    {
        var command = new UpdatePropertyPriceCommand(id, request.Price);
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

        return Ok(new
        {
            success = true,
            message = "Property price updated successfully",
            statusCode = 200
        });
    }

    /// <summary>
    /// Agregar una imagen a una propiedad
    /// </summary>
    /// <param name="id">ID de la propiedad</param>
    /// <param name="request">Datos de la imagen</param>
    /// <returns>ID de la imagen creada</returns>
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddPropertyImage(Guid id, [FromBody] AddPropertyImageRequest request)
    {
        var command = new AddPropertyImageCommand(id, request.File, request.Enabled);
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

        return Ok(new
        {
            success = true,
            data = result.Value,
            statusCode = 200
        });
    }
}

public class AddPropertyImageRequest
{
    public string File { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}

public class UpdatePropertyPriceRequest
{
    public decimal Price { get; set; }
}

public class UpdatePropertyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Year { get; set; }
}
