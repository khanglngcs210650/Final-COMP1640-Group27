namespace Application.Features.Faculties.Queries.ListFaculty;

public class ListFacultyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }


}