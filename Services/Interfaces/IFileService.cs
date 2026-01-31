namespace Midyaf.Services.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
    bool DeleteFile(string fileName, string folderName);
}
