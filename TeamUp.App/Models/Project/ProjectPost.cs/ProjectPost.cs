using Models;

namespace Models;

public partial class ProjectPost
{
    public int Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public string LearningGoals { get; set; } = string.Empty;
    public string TeamAndRols { get; set; } = string.Empty;
    public List<Skill> RequiredSkills { get; private set; } = [];
    public List<ProjectJoinRequest> ProjectJoinRequests { get; private set; } = [];
    public TimeSpan ExpextedDuration { get; set; } 
    public int ExpectedTeamSize { get; set; }
    public Project? Project { get; private set; } = null;

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

        Project = Project.Create(this, acceptedUsers);
        IsStarted = true;

        return true;
    }

    public ProjectPost(User creator,
                       string title,
                       string summary,
                       TimeSpan expextedDuration,
                       int expectedTeamSize,
                       string scenario,
                       string learningGoals,
                       string teamAndRoles,
                       List<Skill> skills)
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
    }

    protected ProjectPost(){} // for ef core
}