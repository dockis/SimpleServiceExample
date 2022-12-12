using SimpleService.Exceptions;
using SimpleService.Models;
using SimpleService.Repositories;

namespace SimpleService.Services;

public class DocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly ICacheRepository _cacheRepository;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        IDocumentRepository repository, 
        ICacheRepository cacheRepository,
        ILogger<DocumentService> logger
        )
    {
        _repository = repository;
        _cacheRepository = cacheRepository;
        _logger = logger;
    }
    
    public void Persist(Document document)
    {
        try
        {
            _repository.Persist(document);
        }
        catch (RepositoryOperationException e)
        {
            var msg = $"Persist document has failed. Document with id ({document.Id}) exist.";
            _logger.LogDebug(e, msg);
            throw new DocumentOperationException(msg, e);
        }
        catch (RuntimeException e)
        {
            _logger.LogDebug(e, $"Persist document has failed. Document with id ({document.Id}) exist.");
            throw;
        }
    }
    
    public void Update(Document document)
    {
        try
        {
            _repository.Update(document);
        }
        catch (RepositoryOperationException e)
        {
            var msg = $"Update document has failed. Document with id ({document.Id}) does not exist.";
            _logger.LogDebug(e, msg);
            throw new DocumentOperationException(msg, e);
        }
        catch (RuntimeException e)
        {
            _logger.LogDebug(e, $"Update document has failed. Document with id ({document.Id}) does not exist.");
            throw;
        }

        if (_cacheRepository.Get(document.Id) is not null)
        {
            _cacheRepository.Set(document);
        }
    }
    
    public Document? GetDocument(string id)
    {
        var document = _cacheRepository.Get(id);
        if (document is not null)
        {
            return document;
        }
        
        document = _repository.GetDocumentById(id);
        if (document is not null)
        {
            _cacheRepository.Set(document);
        }

        return document;
    }
}