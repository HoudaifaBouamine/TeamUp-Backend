using System.ComponentModel.DataAnnotations;

namespace Models;

public partial class Project
{
    [Key]
    public int Id { get; set;}
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty ;
    public DateOnly StartDate { get; set;}
    public DateOnly? EndDate { get; set;} = null;
    public ChatRoom ChatRoom { get; set;} = null!;
    public int TeamSize { get; private set; } = 0;

    public int ProjectPostId { get; init;}
    public ProjectPost ProjectPost { get; init; } = null!;
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
        ProjectsUsers = [new UsersProject
        {
            UserId = creator.Id,
            ProjectId = Id,
            IsMentor = true
        }];
    }

    public static Project Create(ProjectPost projectPost, List<User> acceptedUsers)
    {
        var project = new Project()
        {
            ChatRoom = new ChatRoom(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ProjectPost = projectPost,
            Users = [projectPost.Creator, ..acceptedUsers],
        };

        project.ProjectsUsers.Add(new UsersProject
        {
            User = projectPost.Creator,
            Project = project,
            IsMentor = true
        });

        project.ProjectsUsers.AddRange(acceptedUsers.Select(u => new UsersProject
        {
            User = u,
            Project = project,
            IsMentor = false,
            
        }).ToList());

        return project;
    }

    public static Project Create(string name, string description, DateOnly startDate , User creator)
    {
        return new Project(name, description, startDate, creator);
    }
}