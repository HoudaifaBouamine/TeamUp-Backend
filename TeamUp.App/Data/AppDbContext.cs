using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using TeamUp;
public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {


        builder.Entity<Project>()
            .HasOne(p => p.ChatRoom)
            .WithOne(ch => ch.Project)
            .HasForeignKey<Project>("ChatRoom_Id");

        // builder.Entity<User>()
        //     .HasMany(u=>u.Projects)
        //     .WithMany(p=>p.Users)
        //     .UsingEntity<UsersProject>(
        //             l => l.HasOne<Project>().WithMany(p=>p.ProjectsUsers),
        //             r => r.HasOne<User>().WithMany(u=>u.UsersProjects)
        //         );


        //
    

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

        base.OnModelCreating(builder);
    }

    // Users table is declared in the base class 'IdentityDbContext<User>'
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<ProjectPost> ProjectPosts { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<UsersProject> UsersProjects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }

    // NOTE (HOUDAIFA) : No need for users table declaration because it is already defined inside the base class => IdentityDbContext<User>  => ok 
    // public DbSet<User> Users { get; set; } 



}
