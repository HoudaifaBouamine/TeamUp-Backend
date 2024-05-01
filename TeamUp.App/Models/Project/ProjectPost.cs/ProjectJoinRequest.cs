using Models;

namespace Models;

public partial class ProjectJoinRequest
{
    public int Id { get; private init; }
    public User User { get; private init; } = null!;
    public ProjectPost Project { get; private init; } = null!;
    public bool IsAccepted { get; private set; } = false;
    public bool IsClosed { get; private set; } = false;
    public DateTime CreatedAt { get; private init; }
    public DateTime? RespondAt { get; private set; } = null;
}


partial class ProjectJoinRequest
{

    public static ProjectJoinRequest Create(User user, ProjectPost project)
    {
        return new ProjectJoinRequest
        {
            User = user,
            Project = project,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public bool Accept()
    {
        if(IsClosed) return false;

        IsAccepted = true;
        IsClosed = true;
        RespondAt = DateTime.UtcNow;

        return true;
    }

    public bool Refuse()
    {
        if(IsClosed) return false;

        IsAccepted = false;
        IsClosed = true;
        RespondAt = DateTime.UtcNow;
        
        return true;
    }

    protected ProjectJoinRequest() {}
}
