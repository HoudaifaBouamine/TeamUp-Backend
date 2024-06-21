using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class UserPicture
{
    public Guid Id { get; set; }

    public Guid PictureDataId { get; set; }

    public Guid UserId { get; set; }

    public virtual Picture PictureData { get; set; }

    public virtual AspNetUser User { get; set; }
}
