using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class Application
{
    public long Id { get; set; }

    public long CandidateId { get; set; }

    public long JobPostId { get; set; }

    public long ResumeId { get; set; }

    public string? CoverLetter { get; set; }

    public string Status { get; set; } = null!;

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ApplicationHistory> ApplicationHistories { get; set; } = new List<ApplicationHistory>();

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual JobPost JobPost { get; set; } = null!;

    public virtual Resume Resume { get; set; } = null!;
}
