using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class ChatRoom
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Project Project { get; set; }
}
