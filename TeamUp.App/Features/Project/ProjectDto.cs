namespace Features.Projects;

public record ProjectCreateDto
(
    string Name,
    string Description,
    DateOnly StartDate
);

public record ProjectReadDto
(
    int Id,
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    int UsersCount,
    List<ProjecUserShortDto> UsersSample
);

public record ProjecUserShortDto
(
    string Id, 
    string ProfilePicture
);

public record ProjectDetailsReadDto
(
    int Id,
    string Name,
    string Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    int UsersCount,
    List<ProjecUserLongDto> Users
);

public record ProjecUserLongDto
(
    string Id, 
    string DisplayName, 
    string Handler, 
    string ProfilePicture,
    bool IsMentor
);

public record GetProjectsListResponse
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<ProjectReadDto> Projects
);
