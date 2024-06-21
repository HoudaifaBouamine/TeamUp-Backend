using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class Follow
{
    public int Id { get; set; }

    public Guid FollowerId { get; set; }

    public Guid FolloweeId { get; set; }

    public virtual AspNetUser Followee { get; set; }

    public virtual AspNetUser Follower { get; set; }
}
