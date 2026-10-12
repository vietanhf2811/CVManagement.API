using System;
using System.Collections.Generic;

namespace CVManagement.Domain.Entities;

public partial class Interview
{
    public long Id { get; set; }

    public long ApplicationId { get; set; }

    public long InterviewerUserId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public short DurationMinutes { get; set; }

    public string? Location { get; set; }

    public string? MeetingUrl { get; set; }

    public string? Result { get; set; }

    public string? Feedback { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual User InterviewerUser { get; set; } = null!;
}
