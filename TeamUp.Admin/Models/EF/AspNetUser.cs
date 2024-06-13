using System;
using System.Collections.Generic;

namespace TeamUp.Admin.Models.EF;

public partial class AspNetUser
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Handler { get; set; }

    public string DisplayName { get; set; }

    public int? EmailVerificationCodeId { get; set; }

    public int? PasswordRestCodeId { get; set; }

    public string PasswordResetToken { get; set; }

    public float Rate { get; set; }

    public string ProfilePicture { get; set; }

    public string FullAddress { get; set; }

    public bool IsMentor { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual VerificationCode EmailVerificationCode { get; set; }

    public virtual ICollection<FireBaseNotificationSession> FireBaseNotificationSessions { get; set; } = new List<FireBaseNotificationSession>();

    public virtual ICollection<Follow> FollowFollowees { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual VerificationCode PasswordRestCode { get; set; }

    public virtual ICollection<ProjectJoinRequest> ProjectJoinRequests { get; set; } = new List<ProjectJoinRequest>();

    public virtual ICollection<ProjectPost> ProjectPosts { get; set; } = new List<ProjectPost>();

    public virtual ICollection<ProjectReview> ProjectReviews { get; set; } = new List<ProjectReview>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<UserPicture> UserPictures { get; set; } = new List<UserPicture>();

    public virtual ICollection<UsersProject> UsersProjects { get; set; } = new List<UsersProject>();

    public virtual ICollection<UsersReview> UsersReviewReviewedUserIds { get; set; } = new List<UsersReview>();

    public virtual ICollection<UsersReview> UsersReviewReviewerUserIds { get; set; } = new List<UsersReview>();

    public virtual ICollection<UsersSkill> UsersSkills { get; set; } = new List<UsersSkill>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
