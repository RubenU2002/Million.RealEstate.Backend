using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.PropertyImages.Commands;
using Million.Application.PropertyImages.Queries;
using Million.Application.Properties.DTOs;
using Million.Domain.Entities;

namespace Million.Api.Controllers;

[ApiController]
[Route("api/properties/{propertyId:guid}/images")]
public class PropertyImagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileService _fileService;

    public PropertyImagesController(IMediator mediator, IFileService fileService)
    {
        _mediator = mediator;
        _fileService = fileService;
    }

    /// <summary>
    /// Obtener todas las imágenes de una propiedad
    /// </summary>
    /// <param name="propertyId">ID de la propiedad</param>
    /// <returns>Lista de imágenes de la propiedad</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PropertyImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PropertyImageDto>>> GetPropertyImages(Guid propertyId)
    {
        var query = new GetPropertyImagesQuery(propertyId);
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
    /// Agregar una imagen a una propiedad
    /// </summary>
    /// <param name="propertyId">ID de la propiedad</param>
    /// <param name="file">Archivo de imagen</param>
    /// <param name="enabled">Si la imagen está habilitada</param>
    /// <returns>ID de la imagen creada</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddPropertyImage(
        Guid propertyId, 
        IFormFile file, 
        [FromForm] bool enabled = true)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                error = "No file was uploaded",
                statusCode = 400
            });
        }

        using var stream = file.OpenReadStream();
        var uploadResult = await _fileService.SaveFileAsync(stream, file.FileName, file.ContentType);

        if (uploadResult.IsFailed)
        {
            var statusCode = uploadResult.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = uploadResult.GetFirstErrorMessage(),
                statusCode = statusCode
            });
        }

        var fileUrl = _fileService.GetFileUrl(uploadResult.Value);
        var command = new AddPropertyImageCommand(propertyId, fileUrl, enabled);
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            await _fileService.DeleteFileAsync(uploadResult.Value);
            
            var statusCode = result.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = result.GetFirstErrorMessage(),
                statusCode = statusCode
            });
        }

        return CreatedAtAction(nameof(GetPropertyImages), new { propertyId }, new
        {
            success = true,
            data = result.Value,
            statusCode = 201
        });
    }
}
