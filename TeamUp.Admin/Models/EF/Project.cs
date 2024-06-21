using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int ChatRoomId { get; set; }

    public int TeamSize { get; set; }

    public int ProjectPostId { get; set; }

    public virtual ChatRoom ChatRoom { get; set; }

    public virtual ProjectPost ProjectPost { get; set; }

    public virtual ICollection<ProjectReview> ProjectReviews { get; set; } = new List<ProjectReview>();

    public virtual ICollection<UsersProject> UsersProjects { get; set; } = new List<UsersProject>();
}
