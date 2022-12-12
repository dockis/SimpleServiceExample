using Microsoft.Extensions.Logging;
using Moq;
using SimpleService.Exceptions;
using SimpleService.Models;
using SimpleService.Repositories;
using SimpleService.Services;

namespace SimpleService.UnitTests.Services;

[TestFixture]
public class DocumentServiceTests
{
    private DocumentService _documentService;
    private Mock<IDocumentRepository> _repositoryMock;
    private Mock<ICacheRepository> _cacheServiceMock;
    private Mock<ILogger<DocumentService>> _loggerMock;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IDocumentRepository>();
        _cacheServiceMock = new Mock<ICacheRepository>();
        _loggerMock = new Mock<ILogger<DocumentService>>();

        _documentService = new DocumentService(_repositoryMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public void Persist_SuccessfulDocumentPersist_ReturnVoid()
    {
        var document = GetDocument();

        _documentService.Persist(document);

        _repositoryMock.Verify(x => x.Persist(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void Persist_UnsuccessfulDocumentPersistRepositoryHasFailed_ThrowRuntimeException()
    {
        var document = GetDocument();

        _repositoryMock
            .Setup(x => x.Persist(document))
            .Throws(new RuntimeException());

        Assert.That(
            () => _documentService.Persist(document),
            Throws.Exception.TypeOf<RuntimeException>()
        );

        _repositoryMock.Verify(x => x.Persist(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void Persist_UnsuccessfulDocumentPersistDocumentExists_ThrowDocumentOperationException()
    {
        var document = GetDocument();

        _repositoryMock
            .Setup(x => x.Persist(document))
            .Throws(new RepositoryOperationException());

        Assert.That(
            () => _documentService.Persist(document),
            Throws.Exception.TypeOf<DocumentOperationException>()
        );

        _repositoryMock.Verify(x => x.Persist(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void Update_SuccessfulDocumentUpdateWithCacheUpdate_ReturnVoid()
    {
        var document = GetDocument();

        _cacheServiceMock
            .Setup(x => x.Get(document.Id))
            .Returns(document);

        _documentService.Update(document);

        _repositoryMock.Verify(x => x.Update(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Once());
    }

    [Test]
    public void Update_SuccessfulDocumentUpdateWithoutCacheUpdate_ReturnVoid()
    {
        var document = GetDocument();

        _cacheServiceMock
            .Setup(x => x.Get(document.Id))
            .Returns((Document?) null);

        _documentService.Update(document);

        _repositoryMock.Verify(x => x.Update(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void Update_UnsuccessfulDocumentUpdate_ThrowRuntimeException()
    {
        var document = GetDocument();

        _repositoryMock
            .Setup(x => x.Update(document))
            .Throws(new RuntimeException());

        Assert.That(
            () => _documentService.Update(document),
            Throws.Exception.TypeOf<RuntimeException>()
        );

        _repositoryMock.Verify(x => x.Update(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void Update_UnsuccessfulDocumentUpdateDocumentDoesNotExist_ThrowDocumentOperationException()
    {
        var document = GetDocument();

        _repositoryMock
            .Setup(x => x.Update(document))
            .Throws(new RepositoryOperationException());

        Assert.That(
            () => _documentService.Update(document),
            Throws.Exception.TypeOf<DocumentOperationException>()
        );

        _repositoryMock.Verify(x => x.Update(document), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());
    }

    [Test]
    public void GetDocument_SuccessfulGetDocumentFromRepositoryAndSaveToCache_ReturnDocument()
    {
        var document = GetDocument();

        _cacheServiceMock
            .Setup(x => x.Get(document.Id))
            .Returns((Document?) null);

        _repositoryMock
            .Setup(x => x.GetDocumentById(document.Id))
            .Returns(document);

        var result = _documentService.GetDocument(document.Id);

        _repositoryMock.Verify(x => x.GetDocumentById(document.Id), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Once());

        Assert.That(result, Is.EqualTo(document));
    }

    [Test]
    public void GetDocument_SuccessfulGetDocumentFromCache_ReturnDocument()
    {
        var document = GetDocument();

        _cacheServiceMock
            .Setup(x => x.Get(document.Id))
            .Returns(document);

        var result = _documentService.GetDocument(document.Id);

        _repositoryMock.Verify(x => x.GetDocumentById(document.Id), Times.Never());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());

        Assert.That(result, Is.EqualTo(document));
    }

    [Test]
    public void GetDocument_DocumentHasNotFound_ReturnNull()
    {
        var document = GetDocument();

        _cacheServiceMock
            .Setup(x => x.Get(document.Id))
            .Returns((Document?) null);

        _repositoryMock
            .Setup(x => x.GetDocumentById(document.Id))
            .Returns((Document?) null);

        var result = _documentService.GetDocument(document.Id);

        _repositoryMock.Verify(x => x.GetDocumentById(document.Id), Times.Once());
        _cacheServiceMock.Verify(x => x.Set(document), Times.Never());

        Assert.That(result, Is.Null);
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