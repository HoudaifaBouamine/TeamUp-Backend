using Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FireBaseNotificationSession>(b =>
        {
            b.HasOne<User>().WithMany().HasForeignKey(s => s.UserId);
        });

        builder.Entity<UserPicture>(b =>
        {
            b.Property(x => x.PictureDataId).IsRequired();
            b.HasOne<Picture>().WithMany().HasForeignKey(o => o.PictureDataId).IsRequired();
            b.HasOne<User>().WithMany().HasForeignKey(o => o.UserId).IsRequired();
        });

        builder.Entity<Project>()
            .HasOne(p => p.ChatRoom)
            .WithOne(ch => ch.Project)
            .HasForeignKey<Project>(p=>p.ChatRoomId);

        builder.Entity<UsersProject>()
            .HasOne(up => up.User)
            .WithMany(u => u.UsersProjects)
            .HasForeignKey(up => up.UserId);

        builder.Entity<UsersProject>()
            .HasOne(up => up.Project)
            .WithMany(p => p.ProjectsUsers)
            .HasForeignKey(up => up.ProjectId);

        builder.Entity<User>()
            .HasMany(u => u.Projects)
            .WithMany(p => p.Users)
            .UsingEntity<UsersProject>(
                j => j
                    .HasOne(up => up.Project)
                    .WithMany(p => p.ProjectsUsers)
                    .HasForeignKey(up => up.ProjectId),
                j => j
                    .HasOne(up => up.User)
                    .WithMany(u => u.UsersProjects)
                    .HasForeignKey(up => up.UserId)
            );

        builder.Entity<Message>(m =>
        {
            m.HasOne<ChatRoom>()
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatRoomId);

            m.HasOne<User>()
                .WithMany()
                .HasForeignKey(m=>m.UserId);
        });

        base.OnModelCreating(builder);
    }

    // Users table is declared in the base class 'IdentityDbContext<User>'
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<ProjectPost> ProjectPosts { get; set; }
    public DbSet<ProjectJoinRequest> ProjectJoinRequests { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }

    public DbSet<UsersProject> UsersProjects { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<ProjectReview> ProjectViews {get ; set ; } 
    public DbSet<UserReview> UserReviews {get ; set ; } 
    public DbSet<Picture> Pictures {get ; set ; } 
    public DbSet<UserPicture> UserPictures {get ; set ; } 
    public DbSet<Follow> Follows {get ; set ; } 
    public DbSet<FireBaseNotificationSession> FireBaseNotificationSessions { get ; set ; } 
    
}
