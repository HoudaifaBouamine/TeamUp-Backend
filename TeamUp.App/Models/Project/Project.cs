using System.ComponentModel.DataAnnotations;

namespace Models;

public partial class Project
{
    public int Id { get; set;}
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty ;
    public DateOnly StartDate { get; set;}
    public DateOnly? EndDate { get; set;} = null;
    public ChatRoom ChatRoom { get; set;} = null!;
    public int TeamSize { get; set; } = default;
    public List<User> Users { get; set;} = [];
    public List<UsersProject> ProjectsUsers { get; set; } = [];

}

partial class Project
{
    protected Project() {}

    public Project(string name, string description, DateOnly startDate , User creator)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        ChatRoom = new ChatRoom();
        Users = [creator];
        ProjectsUsers = [];
    }
}