namespace SimpleService.Repositories.Providers;

public interface IFileSystemProvider
{
    public void CreateStorageDirectory(string StorageName);
    public bool IsFileExists(string filename, string StorageName);
    public void Write(string filename, string StorageName, string content);
    public string Read(string filename, string StorageName);
}