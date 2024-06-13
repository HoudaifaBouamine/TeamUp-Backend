using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TeamUp.Admin.Models.EF;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<FireBaseNotificationSession> FireBaseNotificationSessions { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectJoinRequest> ProjectJoinRequests { get; set; }

    public virtual DbSet<ProjectPost> ProjectPosts { get; set; }

    public virtual DbSet<ProjectReview> ProjectReviews { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<UserPicture> UserPictures { get; set; }

    public virtual DbSet<UsersProject> UsersProjects { get; set; }

    public virtual DbSet<UsersReview> UsersReviews { get; set; }

    public virtual DbSet<UsersSkill> UsersSkills { get; set; }

    public virtual DbSet<VerificationCode> VerificationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.EmailVerificationCodeId, "IX_AspNetUsers_EmailVerificationCodeId");

            entity.HasIndex(e => e.PasswordRestCodeId, "IX_AspNetUsers_PasswordRestCodeId");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DisplayName).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Handler).IsRequired();
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.ProfilePicture).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasOne(d => d.EmailVerificationCode).WithMany(p => p.AspNetUserEmailVerificationCodes).HasForeignKey(d => d.EmailVerificationCodeId);

            entity.HasOne(d => d.PasswordRestCode).WithMany(p => p.AspNetUserPasswordRestCodes).HasForeignKey(d => d.PasswordRestCodeId);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Categories_Name");

            entity.HasIndex(e => e.ProjectPostId, "IX_Categories_ProjectPostId");

            entity.HasIndex(e => e.UserId, "IX_Categories_UserId");

            entity.Property(e => e.Name).IsRequired();

            entity.HasOne(d => d.ProjectPost).WithMany(p => p.Categories).HasForeignKey(d => d.ProjectPostId);

            entity.HasOne(d => d.User).WithMany(p => p.Categories).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<FireBaseNotificationSession>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_FireBaseNotificationSessions_UserId");

            entity.Property(e => e.SessionToken).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.FireBaseNotificationSessions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasIndex(e => e.FolloweeId, "IX_Follows_FolloweeId");

            entity.HasIndex(e => e.FollowerId, "IX_Follows_FollowerId");

            entity.HasOne(d => d.Followee).WithMany(p => p.FollowFollowees).HasForeignKey(d => d.FolloweeId);

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers).HasForeignKey(d => d.FollowerId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(e => e.ChatRoomIdId, "IX_Messages_ChatRoomIdId");

            entity.HasIndex(e => e.UserIdid, "IX_Messages_UserIDId");

            entity.Property(e => e.Pinned).HasColumnName("pinned");
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.UserIdid).HasColumnName("UserIDId");

            entity.HasOne(d => d.ChatRoomId).WithMany(p => p.Messages).HasForeignKey(d => d.ChatRoomIdId);

            entity.HasOne(d => d.UserId).WithMany(p => p.Messages).HasForeignKey(d => d.UserIdid);
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bytes).IsRequired();
            entity.Property(e => e.ContentType).IsRequired();
            entity.Property(e => e.FileExtension).IsRequired();
            entity.Property(e => e.FileName).IsRequired();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(e => e.ChatRoomId, "IX_Projects_ChatRoom_Id").IsUnique();

            entity.HasIndex(e => e.ProjectPostId, "IX_Projects_ProjectPostId").IsUnique();

            entity.Property(e => e.ChatRoomId).HasColumnName("ChatRoom_Id");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasOne(d => d.ChatRoom).WithOne(p => p.Project).HasForeignKey<Project>(d => d.ChatRoomId);

            entity.HasOne(d => d.ProjectPost).WithOne(p => p.Project).HasForeignKey<Project>(d => d.ProjectPostId);
        });

        modelBuilder.Entity<ProjectJoinRequest>(entity =>
        {
            entity.HasIndex(e => e.ProjectPostId, "IX_ProjectJoinRequests_ProjectPostId");

            entity.HasIndex(e => e.UserId, "IX_ProjectJoinRequests_UserId");

            entity.Property(e => e.JoinMessage).IsRequired();

            entity.HasOne(d => d.ProjectPost).WithMany(p => p.ProjectJoinRequests).HasForeignKey(d => d.ProjectPostId);

            entity.HasOne(d => d.User).WithMany(p => p.ProjectJoinRequests).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ProjectPost>(entity =>
        {
            entity.HasIndex(e => e.CreatorId, "IX_ProjectPosts_CreatorId");

            entity.Property(e => e.ExpextedDuration).IsRequired();
            entity.Property(e => e.LearningGoals).IsRequired();
            entity.Property(e => e.Scenario).IsRequired();
            entity.Property(e => e.Summary).IsRequired();
            entity.Property(e => e.TeamAndRols).IsRequired();
            entity.Property(e => e.Title).IsRequired();

            entity.HasOne(d => d.Creator).WithMany(p => p.ProjectPosts).HasForeignKey(d => d.CreatorId);
        });

        modelBuilder.Entity<ProjectReview>(entity =>
        {
            entity.HasIndex(e => e.ReviewedProjectId, "IX_ProjectReviews_ReviewedProjectId");

            entity.HasIndex(e => e.ReviewerUserId1, "IX_ProjectReviews_ReviewerUserId1");

            entity.Property(e => e.Text).IsRequired();

            entity.HasOne(d => d.ReviewedProject).WithMany(p => p.ProjectReviews).HasForeignKey(d => d.ReviewedProjectId);

            entity.HasOne(d => d.ReviewerUserId1Navigation).WithMany(p => p.ProjectReviews).HasForeignKey(d => d.ReviewerUserId1);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.HasIndex(e => e.ProjectPostId, "IX_Skills_ProjectPostId");

            entity.HasIndex(e => e.UserId, "IX_Skills_UserId");

            entity.HasOne(d => d.ProjectPost).WithMany(p => p.Skills).HasForeignKey(d => d.ProjectPostId);

            entity.HasOne(d => d.User).WithMany(p => p.Skills).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserPicture>(entity =>
        {
            entity.HasIndex(e => e.PictureDataId, "IX_UserPictures_PictureDataId");

            entity.HasIndex(e => e.UserId, "IX_UserPictures_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.PictureData).WithMany(p => p.UserPictures).HasForeignKey(d => d.PictureDataId);

            entity.HasOne(d => d.User).WithMany(p => p.UserPictures).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UsersProject>(entity =>
        {
            entity.HasIndex(e => e.ProjectId, "IX_UsersProjects_ProjectId");

            entity.HasIndex(e => e.UserId, "IX_UsersProjects_UserId");

            entity.HasOne(d => d.Project).WithMany(p => p.UsersProjects).HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.User).WithMany(p => p.UsersProjects).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UsersReview>(entity =>
        {
            entity.HasIndex(e => e.ReviewedUserIdId, "IX_UsersReviews_ReviewedUserIdId");

            entity.HasIndex(e => e.ReviewerUserIdId, "IX_UsersReviews_ReviewerUserIdId");

            entity.Property(e => e.Stars).HasColumnName("stars");
            entity.Property(e => e.Text).IsRequired();

            entity.HasOne(d => d.ReviewedUserId).WithMany(p => p.UsersReviewReviewedUserIds).HasForeignKey(d => d.ReviewedUserIdId);

            entity.HasOne(d => d.ReviewerUserId).WithMany(p => p.UsersReviewReviewerUserIds).HasForeignKey(d => d.ReviewerUserIdId);
        });

        modelBuilder.Entity<UsersSkill>(entity =>
        {
            entity.HasIndex(e => e.SkillName, "IX_UsersSkills_SkillName");

            entity.HasIndex(e => e.UserId, "IX_UsersSkills_UserId");

            entity.HasOne(d => d.SkillNameNavigation).WithMany(p => p.UsersSkills).HasForeignKey(d => d.SkillName);

            entity.HasOne(d => d.User).WithMany(p => p.UsersSkills).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
