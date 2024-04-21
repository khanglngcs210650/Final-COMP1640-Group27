namespace Application.Features.Auth.Queries.GetSelfProfile
{
    public class GetSelfProfileDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public int NumberOfContributions { get; set; }

        public string Role { get; set; }

        public string FacultyName { get; set; }

        public string AvatarUrl { get; set; }

    }
}
