using Newtonsoft.Json;
using SimpleService.Exceptions;
using SimpleService.Models;
using SimpleService.Repositories.Providers;

namespace SimpleService.Repositories;

public class FileSystemDocumentRepository : IDocumentRepository
{
    private const string StorageName = "DocumentStorage";

    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ILogger<FileSystemDocumentRepository> _logger;
    private readonly string _storageDirectory;

    public FileSystemDocumentRepository(
        IFileSystemProvider fileSystemProvider,
        ILogger<FileSystemDocumentRepository> logger
    )
    {
        _fileSystemProvider = fileSystemProvider;
        _logger = logger;
        _fileSystemProvider.CreateStorageDirectory(StorageName);
    }

    public void Persist(Document document)
    {
        if (_fileSystemProvider.IsFileExists(document.Id, StorageName))
        {
            var msg = $"Persist operation has failed. Document with id ({document.Id}) exists.";
            _logger.LogDebug(msg);
            throw new RepositoryOperationException(msg);
        }

        try
        {
            _fileSystemProvider.Write(document.Id, StorageName, JsonConvert.SerializeObject(document));
        }
        catch (Exception e)
        {
            var msg = "Persist operation has failed.";
            _logger.LogDebug(e, msg);
            throw new RuntimeException(msg, e);
        }
    }

    public void Update(Document document)
    {
        if (!_fileSystemProvider.IsFileExists(document.Id, StorageName))
        {
            var msg = $"Update operation has failed. Document with id ({document.Id}) does not exist.";
            _logger.LogDebug(msg);
            throw new RepositoryOperationException(msg);
        }

        try
        {
            _fileSystemProvider.Write(document.Id, StorageName, JsonConvert.SerializeObject(document));
        }
        catch (Exception e)
        {
            var msg = "Update operation has failed.";
            _logger.LogDebug(e, msg);
            throw new RuntimeException(msg, e);
        }
    }

    public Document? GetDocumentById(string id)
    {
        if (!_fileSystemProvider.IsFileExists(id, StorageName))
        {
            _logger.LogDebug($"Get document operation has failed. Document with id ({id}) does not exist.");
            return null;
        }

        return JsonConvert.DeserializeObject<Document>(_fileSystemProvider.Read(id, StorageName));
    }
}