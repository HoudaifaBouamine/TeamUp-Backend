using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class ProjectJoinRequest
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string JoinMessage { get; set; }

    public int ProjectPostId { get; set; }

    public bool IsAccepted { get; set; }

    public bool IsClosed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RespondAt { get; set; }

    public virtual ProjectPost ProjectPost { get; set; }

    public virtual AspNetUser User { get; set; }
}
