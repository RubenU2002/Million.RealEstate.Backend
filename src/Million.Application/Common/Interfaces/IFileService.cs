using FluentResults;

namespace Million.Application.Common.Interfaces;

public interface IFileService
{
    Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Result> DeleteFileAsync(string filePath);
    Task<Result<byte[]>> GetFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
