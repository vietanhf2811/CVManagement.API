using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class JobPost
{
    public long Id { get; set; }

    public long CreatedByUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Department { get; set; }

    public string? Location { get; set; }

    public string? EmploymentType { get; set; }

    public byte? ExperienceMinYears { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string Currency { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Requirements { get; set; }

    public string? Benefits { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public DateOnly? Deadline { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}
