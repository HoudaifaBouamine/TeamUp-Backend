using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class AspNetRoleClaim
{
    public int Id { get; set; }

    public Guid RoleId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    public virtual AspNetRole Role { get; set; }
}
