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

        base.OnModelCreating(builder);
    }

    // Users table is declared in the base class 'IdentityDbContext<User>'
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<UsersProject> UsersProjects { get; set; }

}
