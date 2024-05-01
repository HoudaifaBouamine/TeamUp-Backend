using Models;

namespace Models;

public class ProjectPost
{
    public int Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public string LearningGoals { get; set; } = string.Empty;
    public string TeamAndRols { get; set; } = string.Empty;
    public List<Skill> RequiredSkills { get; set; } = [];
    public TimeSpan ExpextedDuration { get; set; } 
    public int ExpectedTeamSize { get; set; }
    public Project? Project { get; private init; } = null;
    public User Creator { get; private init; } = null!;

    public ProjectPost(User creator,
                       string title,
                       string summary,
                       TimeSpan expextedDuration,
                       int expectedTeamSize,
                       string scenario,
                       string learningGoals,
                       string teamAndRols,
                       List<Skill> skills)
    {
        this.Creator = creator;
        this.LearningGoals = learningGoals;
        this.Scenario = scenario;
        this.Summary = summary;
        this.Title = title;
        this.TeamAndRols = teamAndRols;
        this.ExpectedTeamSize = expectedTeamSize;
        this.ExpextedDuration = expextedDuration;
        this.RequiredSkills = skills;
    }

    protected ProjectPost(){} // for ef core
}