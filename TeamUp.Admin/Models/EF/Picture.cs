using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class Picture
{
    public Guid Id { get; set; }

    public byte[] Bytes { get; set; }

    public string FileExtension { get; set; }

    public string ContentType { get; set; }

    public decimal Size { get; set; }

    public string FileName { get; set; }

    public virtual ICollection<UserPicture> UserPictures { get; set; } = new List<UserPicture>();
}
