using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class ProjectReview
{
    public int Id { get; set; }

    public string Text { get; set; }

    public short Stars { get; set; }

    public int ReviewerUserId { get; set; }

    public Guid ReviewerUserId1 { get; set; }

    public int ReviewedProjectId { get; set; }

    public virtual Project ReviewedProject { get; set; }

    public virtual AspNetUser ReviewerUserId1Navigation { get; set; }
}
