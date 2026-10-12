using System;
using System.Collections.Generic;

namespace CVManagement.Domain.Entities;

public partial class Resume
{
    public long Id { get; set; }

    public long CandidateId { get; set; }

    public string Title { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string StorageKey { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long? FileSizeBytes { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Candidate Candidate { get; set; } = null!;
}
