using Microsoft.AspNetCore.Mvc;
using SimpleService.Exceptions;
using SimpleService.Models;
using SimpleService.Services;

namespace SimpleService.Controllers;

[ApiController]
[Route("documents")]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _documentService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(
        DocumentService documentService,
        ILogger<DocumentController> logger
    )
    {
        _documentService = documentService;
        _logger = logger;
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult PersistDocument([FromBody] Document document)
    {
        try
        {
            _documentService.Persist(document);
            
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (DocumentOperationException e)
        {
            var msg = $"Document was not persist. Document with id ({document.Id}) already exist.";
            _logger.LogDebug(e, msg);
            return StatusCode(StatusCodes.Status400BadRequest, new {statusCode = StatusCodes.Status400BadRequest, message = msg});
        }
        catch (RuntimeException e)
        {
            _logger.LogError(e, "Persist document has failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new {statusCode = StatusCodes.Status500InternalServerError});
        }
    }

    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult UpdateDocument([FromBody] Document document)
    {
        try
        {
            _documentService.Update(document);
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (DocumentOperationException e)
        {
            var msg = $"Document was not update. Document with id ({document.Id}) does not exist.";
            _logger.LogDebug(e, msg);
            return StatusCode(StatusCodes.Status400BadRequest, new {statusCode = StatusCodes.Status400BadRequest, message = msg});
        }
        catch (RuntimeException e)
        {
            _logger.LogError(e, "Update document has failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new {statusCode = StatusCodes.Status500InternalServerError});
        }
    }

    [HttpGet("{id}")]
    [Consumes("application/json")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetDocument(string id)
    {
        Document? document = null;
        try
        {
            document = _documentService.GetDocument(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get document has failed. Document id ({document.Id})");
            return StatusCode(StatusCodes.Status500InternalServerError, new {statusCode = StatusCodes.Status500InternalServerError});
        }

        if (document is null)
        {
            var msg = $"Document was not found. Document with id ({document.Id}) does not exist.";
            _logger.LogDebug(msg);
            return StatusCode(StatusCodes.Status404NotFound, new {statusCode = StatusCodes.Status404NotFound, message = msg});
        }

        return Ok(document);
    }
}