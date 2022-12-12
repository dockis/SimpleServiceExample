namespace SimpleService.Repositories.Providers;

public class FileSystemProvider : IFileSystemProvider
{
    private readonly string _storagePath = Environment.CurrentDirectory;
    
    public void CreateStorageDirectory(string StorageName)
    {
        var path = Path.Join(_storagePath, StorageName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public bool IsFileExists(string filename, string StorageName)
    {
        return File.Exists(Path.Join(_storagePath, StorageName, filename));
    }

    public void Write(string filename, string StorageName, string content)
    {
        File.WriteAllText(Path.Join(_storagePath, StorageName, filename), content);
    }

    public string Read(string filename, string StorageName)
    {
        return File.ReadAllText(Path.Join(_storagePath, StorageName, filename));
    }
}