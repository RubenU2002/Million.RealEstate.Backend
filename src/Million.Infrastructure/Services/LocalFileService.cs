using FluentResults;
using Microsoft.Extensions.Configuration;
using Million.Application.Common.Interfaces;

namespace Million.Infrastructure.Services;

public class LocalFileService : IFileService
{
    private readonly string _uploadsPath;
    private readonly string _baseUrl;

    public LocalFileService(IConfiguration configuration)
    {
        _uploadsPath = configuration["FileStorage:UploadsPath"] ?? "wwwroot/uploads";
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "http://localhost:5233";
        
        Directory.CreateDirectory(_uploadsPath);
    }

    public async Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(contentType.ToLower()))
            {
                return Result.Fail($"File type {contentType} is not allowed. Only images are supported.");
            }

            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            return Result.Ok(uniqueFileName);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error saving file: {ex.Message}");
        }
    }

    public async Task<Result> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadsPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting file: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> GetFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadsPath, filePath);
            if (!File.Exists(fullPath))
            {
                return Result.Fail("File not found");
            }

            var bytes = await File.ReadAllBytesAsync(fullPath);
            return Result.Ok(bytes);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error reading file: {ex.Message}");
        }
    }

    public string GetFileUrl(string filePath)
    {
        return $"{_baseUrl}/uploads/{filePath}";
    }
}
