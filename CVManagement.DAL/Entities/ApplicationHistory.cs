using System;
using System.Collections.Generic;

namespace CVManagement.DAL.Entities;

public partial class ApplicationHistory
{
    public long Id { get; set; }

    public long ApplicationId { get; set; }

    public long ChangedByUserId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual User ChangedByUser { get; set; } = null!;
}
