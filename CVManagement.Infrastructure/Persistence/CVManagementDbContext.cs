
using System;
using System.Collections.Generic;
using CVManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using ApplicationEntity = global::CVManagement.Domain.Entities.Application;

namespace CVManagement.Infrastructure.Persistence;

public partial class CVManagementDbContext : DbContext
{
    public CVManagementDbContext(DbContextOptions<CVManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationEntity> Applications { get; set; }

    public virtual DbSet<ApplicationHistory> ApplicationHistories { get; set; }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<CandidateSkill> CandidateSkills { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<JobPost> JobPosts { get; set; }

    public virtual DbSet<JobSkill> JobSkills { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationEntity>(entity =>
        {
            entity.HasIndex(e => new { e.CandidateId, e.AppliedAt }, "IX_Applications_Candidate_AppliedAt").IsDescending(false, true);

            entity.HasIndex(e => new { e.JobPostId, e.Status, e.AppliedAt }, "IX_Applications_JobPost_Status_AppliedAt").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.CandidateId, e.JobPostId }, "UQ_Applications_Candidate_Job").IsUnique();

            entity.Property(e => e.AppliedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Submitted");
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Candidate).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Candidates");

            entity.HasOne(d => d.JobPost).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_JobPosts");

            entity.HasOne(d => d.Resume).WithMany(p => p.Applications)
                .HasPrincipalKey(p => new { p.Id, p.CandidateId })
                .HasForeignKey(d => new { d.ResumeId, d.CandidateId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Resumes_Candidate");
        });

        modelBuilder.Entity<ApplicationHistory>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.ChangedAt }, "IX_ApplicationHistories_ApplicationId_ChangedAt").IsDescending(false, true);

            entity.Property(e => e.ChangedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.NewStatus).HasMaxLength(30);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.OldStatus).HasMaxLength(30);

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationHistories)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationHistories_Applications");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.ApplicationHistories)
                .HasForeignKey(d => d.ChangedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationHistories_Users");
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UQ_Candidates_UserId").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LinkedInUrl).HasMaxLength(500);
            entity.Property(e => e.PortfolioUrl).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.User).WithOne(p => p.Candidate)
                .HasForeignKey<Candidate>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidates_Users");
        });

        modelBuilder.Entity<CandidateSkill>(entity =>
        {
            entity.HasKey(e => new { e.CandidateId, e.SkillId });

            entity.Property(e => e.ProficiencyLevel).HasMaxLength(30);
            entity.Property(e => e.YearsOfExperience).HasColumnType("decimal(4, 1)");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateSkills_Candidates");

            entity.HasOne(d => d.Skill).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateSkills_Skills");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.ScheduledAt }, "IX_Interviews_ApplicationId_ScheduledAt");

            entity.HasIndex(e => new { e.InterviewerUserId, e.ScheduledAt }, "IX_Interviews_Interviewer_ScheduledAt");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DurationMinutes).HasDefaultValue((short)60);
            entity.Property(e => e.Location).HasMaxLength(300);
            entity.Property(e => e.MeetingUrl).HasMaxLength(1000);
            entity.Property(e => e.Result).HasMaxLength(30);
            entity.Property(e => e.ScheduledAt).HasPrecision(0);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interviews_Applications");

            entity.HasOne(d => d.InterviewerUser).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.InterviewerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interviews_Users");
        });

        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasIndex(e => new { e.Status, e.Deadline }, "IX_JobPosts_Status_Deadline");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("VND")
                .IsFixedLength();
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.PublishedAt).HasPrecision(0);
            entity.Property(e => e.SalaryMax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalaryMin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobPosts_Users");
        });

        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(e => new { e.JobPostId, e.SkillId });

            entity.Property(e => e.IsRequired).HasDefaultValue(true);

            entity.HasOne(d => d.JobPost).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.JobPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobSkills_JobPosts");

            entity.HasOne(d => d.Skill).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobSkills_Skills");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasIndex(e => new { e.CandidateId, e.CreatedAt }, "IX_Resumes_CandidateId_CreatedAt").IsDescending(false, true);

            entity.HasIndex(e => new { e.Id, e.CandidateId }, "UQ_Resumes_Id_CandidateId").IsUnique();

            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .HasDefaultValue("application/pdf");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OriginalFileName).HasMaxLength(255);
            entity.Property(e => e.StorageKey).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Candidate).WithMany(p => p.Resumes)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resumes_Candidates");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Skills_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.UpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });
        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
