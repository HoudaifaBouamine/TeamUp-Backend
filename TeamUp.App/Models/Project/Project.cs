using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.Marshalling;
using MimeKit.IO.Filters;

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
    public int TeamSize { get; private set; } = 0;

    // private List<User> _users { get; set;} = [];
    // public IEnumerable<User> Users => _users.AsReadOnly();

    // private List<UsersProject> _projectsUsers { get; set; } = [];
    // public IEnumerable<UsersProject> ProjectsUsers => _projectsUsers.AsReadOnly();



    public List<User> Users { get; set; } = [];
    public List<UsersProject> ProjectsUsers { get; set; } = [];

}

public partial class Project
{

    public void AddUser(User user, bool isMentor = false)
    {
        Users.Add(user);
        ProjectsUsers.Add(new UsersProject
        {
            User = user,
            IsMentor = isMentor
        });
        
        TeamSize++;
    }

    public void AddUsers(IEnumerable<User> user)
    {
        Users.AddRange(user);
        Users = Users.DistinctBy(u=>u.Id).ToList();
        TeamSize = user.Count();
    }
    public Project() {}

    private Project(string name, string description, DateOnly startDate , User creator)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        ChatRoom = new ChatRoom();
        Users = [creator];
        ProjectsUsers = [];
    }

    public static Project Create(string name, string description, DateOnly startDate , User creator)
    {
        return new Project(name, description, startDate, creator);
    }
}