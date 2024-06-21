using System.ComponentModel.DataAnnotations;

namespace Models;

// This is the join table between Users & Projects table, allowing to perform M-TO-M relationship
// [Table("UsersProjects")] NOTE : The default name of the table will be the same name of the declared collection on the AppDbContext, if the table and collection names are the same, no need for addional information
public class UsersProject
{
    [Key]
    public int Id { get; set; }
    public User User { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Guid UserId { get; set; }
    public int ProjectId { get; set; }
    public bool IsMentor { get; set; } = false;
}


