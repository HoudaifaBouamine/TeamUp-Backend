﻿using Models;

namespace TeamUp.Features.Project;

public interface IProjectRepository
{
    Task<ProjectReadDto> GetByIdAsync(int id);
    Task<GetProjectsListResponse> GetListWithSearchAndPaginationAsync (
        int? PageSize,int? PageNumber, string? SearchPattern);
    Task<GetProjectsListResponse> GetListWithFiltersAsync (
        int? PageSize,int? PageNumber, string? SearchPattern, 
        string[]? TeamSizes, string[]? Categories, string[]? Durations);
    Task<int> CreateAsync(ProjectCreateDto projectDto,User user);
    Task <bool>UpdateAsync(int id, ProjectCreateDto projectDto);
    Task<bool> DeleteAsync(int id);
    Task<int> GetUsersCountAsync(int projectId);
    Task<IEnumerable<ProjecUserShortDto>> GetUsersSampleAsync(int projectId);
    Task<ProjectDetailsReadDto?> GetDetailsAsync(int projectId);

    Task<bool> AddUserToProjectAsync(int projectId, Guid userId, bool isMentor);

    Task<GetProjectsListResponse2> GetListWithSearchAndPagination2Async (int? pageSize,int? pageNumber, string? SearchPattern);
    Task<GetProjectsListResponse4> GetListWithSearchAndPagination4Async (int? pageSize,int? pageNumber, string? searchPattern);
    Task<GetProjectsListResponse4> GetUsersProjectsListWithSearchAndPaginationAsync (Guid userId ,int? pageSize,int? pageNumber, string? searchPattern);
    Task<bool> CheckMentor(Guid userId, int projectId);
}

#region V1

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
    Guid Id, 
    string ProfilePicture
);

public record ProjectDetailsReadDto(
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
    Guid Id, 
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

public class GetProjectDetailsResponse
{
    public string SearchPattern { get; set; } = "";
}

#endregion

#region V2

public record GetProjectsListResponse4
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<TeamUp.Features.Project.ProjectsController.ProjectDetailsReadDto> Projects
);

public record GetProjectsListResponse2
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<ProjectReadDto2> Projects
);

public record ProjectReadDto2
(
    int Id,
    string Title,
    string Summary,
    string Scenario,
    string LearningGoals,
    string TeamAndRols,
    List<string> RequiredSkills,
    DateOnly StartDate,
    DateOnly? EndDate,
    int UsersCount,
    List<ProjecUserShortDto> UsersSample
);


#endregion