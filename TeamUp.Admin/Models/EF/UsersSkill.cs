using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class UsersSkill
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string SkillName { get; set; }

    public virtual Skill SkillNameNavigation { get; set; }

    public virtual AspNetUser User { get; set; }
}
