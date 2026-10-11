using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class CandidateSkill
{
    public long CandidateId { get; set; }

    public int SkillId { get; set; }

    public decimal? YearsOfExperience { get; set; }

    public string? ProficiencyLevel { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
