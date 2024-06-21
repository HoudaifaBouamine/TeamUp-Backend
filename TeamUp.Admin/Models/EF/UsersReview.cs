using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class UsersReview
{
    public int Id { get; set; }

    public string Text { get; set; }

    public Guid ReviewerUserIdId { get; set; }

    public Guid ReviewedUserIdId { get; set; }

    public short Stars { get; set; }

    public virtual AspNetUser ReviewedUserId { get; set; }

    public virtual AspNetUser ReviewerUserId { get; set; }
}
