using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class FireBaseNotificationSession
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string SessionToken { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser User { get; set; }
}
