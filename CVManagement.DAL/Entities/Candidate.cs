using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class Candidate
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? PortfolioUrl { get; set; }

    public string? Summary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();

    public virtual User User { get; set; } = null!;
}
