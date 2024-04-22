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

    private List<User> _users { get; set;} = [];

    [NotMapped]
    public IEnumerable<User> Users 
    { 
        get
        {
            return _users.AsReadOnly();
        }
    }

    private List<UsersProject> _projectsUsers { get; set; } = [];

    [NotMapped]
    public IEnumerable<UsersProject> ProjectsUsers 
    {
        get
        {
            return _projectsUsers.AsReadOnly();
        }
    }

}

public partial class Project
{

    public void AddUser(User user, bool isMentor = false)
    {
        _users.Add(user);
        _projectsUsers.Add(new UsersProject
        {
            User = user,
            IsMentor = isMentor
        });
        
        TeamSize++;
    }

    public void AddUsers(IEnumerable<User> user)
    {
        _users.AddRange(user);
        _users = _users.DistinctBy(u=>u.Id).ToList();
        TeamSize = user.Count();
    }
    public Project() {}

    private Project(string name, string description, DateOnly startDate , User creator)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        ChatRoom = new ChatRoom();
        _users = [creator];
        _projectsUsers = [];
    }

    public static Project Create(string name, string description, DateOnly startDate , User creator)
    {
        return new Project(name, description, startDate, creator);
    }
}