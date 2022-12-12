using Microsoft.Extensions.Logging;
using Moq;
using SimpleService.Exceptions;
using SimpleService.Models;
using SimpleService.Repositories;
using SimpleService.Repositories.Providers;

namespace SimpleService.UnitTests.Repositories;

[TestFixture]
public class FileSystemDocumentRepositoryTests
{
    private IDocumentRepository _repository;
    private Mock<IFileSystemProvider> _fileSystemProviderMock;
    private Mock<ILogger<FileSystemDocumentRepository>> _loggerMock;

    [SetUp]
    public void SetUp()
    {
        _fileSystemProviderMock = new Mock<IFileSystemProvider>();
        _loggerMock = new Mock<ILogger<FileSystemDocumentRepository>>();

        _repository = new FileSystemDocumentRepository(_fileSystemProviderMock.Object, _loggerMock.Object);
    }

    [Test]
    public void Persist_SuccessfulDocumentPersist_ReturnVoid()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(false);

        _repository.Persist(document);

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void Persist_UnsuccessfulDocumentPersistFileExists_ThrowRepositoryOperationException()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(true);

        Assert.That(
            () => _repository.Persist(document),
            Throws.Exception.TypeOf<RepositoryOperationException>()
        );

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Test]
    public void Persist_UnsuccessfulDocumentPersistWriteFileHasFailed_ThrowRuntimeException()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(false);

        _fileSystemProviderMock
            .Setup(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new IOException());

        Assert.That(
            () => _repository.Persist(document),
            Throws.Exception.TypeOf<RuntimeException>()
        );

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void Update_SuccessfulDocumentUpdate_ReturnVoid()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(true);

        _repository.Update(document);

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void Update_UnsuccessfulDocumentUpdateFileDoesNotExist_ThrowRepositoryOperationException()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(false);

        Assert.That(
            () => _repository.Update(document),
            Throws.Exception.TypeOf<RepositoryOperationException>()
        );

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Test]
    public void Update_UnsuccessfulDocumentUpdateWriteFileHasFailed_ThrowRuntimeException()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(true);

        _fileSystemProviderMock
            .Setup(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new IOException());

        Assert.That(
            () => _repository.Update(document),
            Throws.Exception.TypeOf<RuntimeException>()
        );

        _fileSystemProviderMock.Verify(x => x.Write(document.Id, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void GetDocumentById_SuccessfulDocumentRead_ReturnDocument()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(true);

        _fileSystemProviderMock
            .Setup(x => x.Read(document.Id, It.IsAny<string>()))
            .Returns("{}");

        _repository.GetDocumentById(document.Id);
        
        _fileSystemProviderMock.Verify(x => x.Read(document.Id, It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void GetDocumentById_DocumentDoesNotExist_ReturnNull()
    {
        var document = GetDocument();

        _fileSystemProviderMock
            .Setup(x => x.IsFileExists(document.Id, It.IsAny<string>()))
            .Returns(false);

        _repository.GetDocumentById(document.Id);
        
        _fileSystemProviderMock.Verify(x => x.Read(document.Id, It.IsAny<string>()), Times.Never());
    }

    private Document GetDocument()
    {
        return new Document()
        {
            Id = "some-id",
            Tags = new[] {"tag-1", "tag-2"},
            Data = new {Some = "data-field"}
        };
    }
}