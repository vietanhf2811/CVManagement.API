
namespace CVManagement.Application.DTOs.Candidates;

public class CandidateProfileDto
{

    public CandidateUserDto User { get; set; } = new();

    public long CandidateId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? PortfolioUrl { get; set; }

    public string? Summary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Related information
    public List<CandidateSkillDto> Skills { get; set; } = new();

    public List<CandidateResumeDto> Resumes { get; set; } = new();
}
