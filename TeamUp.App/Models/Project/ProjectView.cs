using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("ProjectViews")]
public class ProjectView
{
    public int Id { get; set;}
    public string Text { get; set; } = string.Empty;
    public byte stars { get; set; } 
    public User? ReviewerUserId { get; set; } 
    public Project? ReviewedProjectId { get; set; }
}