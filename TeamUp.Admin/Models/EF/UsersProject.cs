using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class UsersProject
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int ProjectId { get; set; }

    public bool IsMentor { get; set; }

    public virtual Project Project { get; set; }

    public virtual AspNetUser User { get; set; }
}
