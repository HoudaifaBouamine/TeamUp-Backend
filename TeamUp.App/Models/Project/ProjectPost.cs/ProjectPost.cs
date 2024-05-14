using System.ComponentModel.DataAnnotations;
using Models;

namespace Models;

public partial class ProjectPost
{
    public int Id { get; init; }
    public DateTime PostingTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public string LearningGoals { get; set; } = string.Empty;
    public string TeamAndRols { get; set; } = string.Empty;
    public List<Skill> RequiredSkills { get; private set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<ProjectJoinRequest> ProjectJoinRequests { get; private set; } = [];

    [AllowedValues("1 Week", "2-3 Weeks", "1 Month", "2-3 Months", "+3 Months")]
    public string ExpextedDuration { get; set; }
    public int ExpectedTeamSize { get; set; }
    public global::Models.Project? Project { get; private set; } = null;

    public Guid CreatorId { get; set; }
    public User Creator { get; private init; } = null!;
    public bool IsStarted {get; private set;} = false;
}


partial class ProjectPost
{

    public bool Start()
    {
        if(IsStarted)
            return false;


        var acceptedUsers = ProjectJoinRequests
            .Where(r => r.IsAccepted)
            .Select(r => r.User)
            .ToList();

        Project = global::Models.Project.Create(this, acceptedUsers);
        IsStarted = true;

        return true;
    }

    public ProjectPost(User creator,
                       string title,
                       string summary,
                       string expextedDuration,
                       int expectedTeamSize,
                       string scenario,
                       string learningGoals,
                       string teamAndRoles,
                       List<Skill> skills,
                       List<Category> categories)
    {
        this.Creator = creator;
        this.LearningGoals = learningGoals;
        this.Scenario = scenario;
        this.Summary = summary;
        this.Title = title;
        this.TeamAndRols = teamAndRoles;
        this.ExpectedTeamSize = expectedTeamSize;
        this.ExpextedDuration = expextedDuration;
        this.RequiredSkills = skills;
        this.Categories = categories;
        this.PostingTime = DateTime.UtcNow;
    }

    private ProjectPost(){} // for ef core
}