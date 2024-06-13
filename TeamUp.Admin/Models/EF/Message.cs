using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; }

    public DateTime DateTime { get; set; }

    public int ChatRoomIdId { get; set; }

    public Guid UserIdid { get; set; }

    public bool Pinned { get; set; }

    public virtual ChatRoom ChatRoomId { get; set; }

    public virtual AspNetUser UserId { get; set; }
}
