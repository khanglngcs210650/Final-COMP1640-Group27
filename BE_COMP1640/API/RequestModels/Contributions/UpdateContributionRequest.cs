namespace API.RequestModels.Contributions;

public class UpdateContributionRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }

    public IFormFile? DocumentFile { get; set; }
}