using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class Skill
{
    public string Name { get; set; }

    public int? ProjectPostId { get; set; }

    public Guid? UserId { get; set; }

    public virtual ProjectPost ProjectPost { get; set; }

    public virtual AspNetUser User { get; set; }

    public virtual ICollection<UsersSkill> UsersSkills { get; set; } = new List<UsersSkill>();
}
