using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Models;

public class AppDbContext : IdentityDbContext<User>
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
    public DbSet<Project> Projects { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<UsersProject> UsersProjects { get; set; }

}
