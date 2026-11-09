
namespace CVManagement.Application.DTOs.Candidates;

public class CandidateResumeDto
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }
}
