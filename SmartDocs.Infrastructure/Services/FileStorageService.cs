using Microsoft.Extensions.Configuration;

namespace SmartDocs.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration config)
    {
        _basePath = config["FileStorage:BasePath"] ?? "uploads";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_basePath, uniqueName);

        using var fileOut = File.Create(fullPath);
        await fileStream.CopyToAsync(fileOut);

        return fullPath;
    }

    public Task DeleteAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }
}