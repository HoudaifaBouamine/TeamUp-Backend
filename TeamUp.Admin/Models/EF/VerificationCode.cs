using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class VerificationCode
{
    public int Id { get; set; }

    public string Code { get; set; }

    public DateTime? ExpireTime { get; set; }

    public int? CodeTriesCount { get; set; }

    public int VerificationCodeType { get; set; }

    public virtual ICollection<AspNetUser> AspNetUserEmailVerificationCodes { get; set; } = new List<AspNetUser>();

    public virtual ICollection<AspNetUser> AspNetUserPasswordRestCodes { get; set; } = new List<AspNetUser>();
}
