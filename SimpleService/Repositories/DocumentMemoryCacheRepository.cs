using Microsoft.Extensions.Caching.Memory;
using SimpleService.Models;

namespace SimpleService.Repositories;

public class DocumentMemoryCacheRepository : ICacheRepository
{
    private const int CacheItemExpiration = 500;
    private const string CacheItemIdPrefix = "document";

    private readonly IMemoryCache _memoryCache;

    public DocumentMemoryCacheRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Document? Get(string id)
    {
        _memoryCache.TryGetValue<Document>($"{CacheItemIdPrefix}-{id}", out var document);

        return document;
    }

    public void Set(Document document)
    {
        _memoryCache.Set(
            $"{CacheItemIdPrefix}-{document.Id}",
            document,
            TimeSpan.FromSeconds(CacheItemExpiration)
        );
    }
}