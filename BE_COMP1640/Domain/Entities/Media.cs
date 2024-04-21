using Domain.Common;

namespace Domain.Entities;

public class Media : BaseEntity
{

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public string? UrlFilePath { get; set; }

    public string? LocalFilePath { get; set; }

    public long? FileSizeInBytes { get; set; }


}

