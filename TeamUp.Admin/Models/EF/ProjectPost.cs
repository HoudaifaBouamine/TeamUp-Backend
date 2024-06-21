using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class ProjectPost
{
    public int Id { get; set; }

    public DateTime PostingTime { get; set; }

    public string Title { get; set; }

    public string Summary { get; set; }

    public string Scenario { get; set; }

    public string LearningGoals { get; set; }

    public string TeamAndRols { get; set; }

    public string ExpextedDuration { get; set; }

    public int ExpectedTeamSize { get; set; }

    public Guid CreatorId { get; set; }

    public bool IsStarted { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual AspNetUser Creator { get; set; }

    public virtual Project Project { get; set; }

    public virtual ICollection<ProjectJoinRequest> ProjectJoinRequests { get; set; } = new List<ProjectJoinRequest>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
