using SimpleService.Models;

namespace SimpleService.Repositories;

public interface ICacheRepository
{
    public Document? Get(string id);
    public void Set(Document document);
}