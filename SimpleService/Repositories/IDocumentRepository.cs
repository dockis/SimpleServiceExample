using SimpleService.Models;

namespace SimpleService.Repositories;

public interface IDocumentRepository
{
    public void Persist(Document document);
    public void Update(Document document);
    public Document? GetDocumentById(string id);
}