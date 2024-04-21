using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace Infrastructure.FileService;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IHttpContextAccessor _contextAccessor;

    public FileService(IWebHostEnvironment hostEnvironment, IHttpContextAccessor contextAccessor)
    {
        _hostEnvironment = hostEnvironment;
        _contextAccessor = contextAccessor;
    }



    public async Task<Media> SaveFileAsync(IFormFile file, string folderName)
    {
        var id = Guid.NewGuid();

        var folderPath = EnsureFolderExists(folderName);
        var filePath = await SaveFileToFolder(file, folderPath, id);

        return CreateMediaEntity(file, folderName, filePath, id);
    }

    public async Task UpdateFileAsync(IFormFile file, string folderName, Media oldFile)
    {
        var folderPath = EnsureFolderExists(folderName);
        var oldFilePath = GetFilePathForMedia(folderName, oldFile);

        DeleteFile(oldFilePath);

        var newFilePath = await SaveFileToFolder(file, folderPath, oldFile.Id);

        UpdateMediaEntity(file, folderName, oldFile, newFilePath);
    }


    public async Task<byte[]> ZipFilesAsync(IEnumerable<Media> mediaList, string folderName)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var media in mediaList)
            {
                var entry = archive.CreateEntry($"{media.FileName}.{media.FileExtension}");
                await using var entryStream = entry.Open();
                await using var fileStream = File.OpenRead(GetFilePathForMedia(folderName, media));
                await fileStream.CopyToAsync(entryStream);
            }
        }

        return memoryStream.ToArray();
    }

    private string EnsureFolderExists(string folderName)
    {
        var folderPath = Path.Combine(_hostEnvironment.ContentRootPath, "MediaFiles", folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    private async Task<string> SaveFileToFolder(IFormFile file, string folderPath, Guid id)
    {
        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{id}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return filePath;
    }

    private void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private string GetFilePathForMedia(string folderName, Media file)
    {
        return Path.Combine(_hostEnvironment.ContentRootPath, "MediaFiles", folderName,
            $"{file.FileName}_{file.Id}{file.FileExtension}");
    }

    private Media CreateMediaEntity(IFormFile file, string folderName, string filePath, Guid id)
    {
        return new Media
        {
            Id = id,
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            FileExtension = Path.GetExtension(file.FileName),
            FileSizeInBytes = file.Length,
            LocalFilePath = filePath,
            UrlFilePath = GetFileUrl(folderName, Path.GetFileName(filePath))
        };
    }

    private void UpdateMediaEntity(IFormFile file, string folderName, Media oldFile, string newFilePath)
    {
        oldFile.FileName = Path.GetFileNameWithoutExtension(file.FileName);
        oldFile.FileExtension = Path.GetExtension(file.FileName);
        oldFile.FileSizeInBytes = file.Length;
        oldFile.LocalFilePath = newFilePath;
        oldFile.UrlFilePath = GetFileUrl(folderName, Path.GetFileName(newFilePath));
    }

    public string GetFileUrl(string folderName, string fileName)
    {
        var baseUrl = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}{_contextAccessor.HttpContext.Request.PathBase}";
        return $"{baseUrl}/MediaFiles/{folderName}/{fileName}";
    }
}