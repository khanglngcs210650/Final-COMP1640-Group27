namespace Application.Common.Models;

public class SuccessResult
{
    public string Title { get; set; }

    public object? Data { get; set; }


    public SuccessResult(string title, object? data = null)
    {
        Title = title;
        Data = data;
    }
}