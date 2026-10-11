using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class User
{
    public long Id { get; set; }

    public int RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ApplicationHistory> ApplicationHistories { get; set; } = new List<ApplicationHistory>();

    public virtual Candidate? Candidate { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    public virtual Role Role { get; set; } = null!;
}
