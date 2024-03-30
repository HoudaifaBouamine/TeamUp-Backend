using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Features.Projects; 

namespace Repositories;

public interface IProjectRepository
{
    Task<ProjectReadDto> GetByIdAsync(int id);
    Task<GetProjectsListResponse> GetAllAsync (int? PageSize,int? PageNumber, string? SearchPattern);
    Task<int> CreateAsync(ProjectCreateDto projectDto,User user);
    Task UpdateAsync(int id, ProjectCreateDto projectDto);
    Task DeleteAsync(int id);
    Task<int> GetUsersCountAsync(int projectId);
    Task<IEnumerable<ProjecUserShortDto>> GetUsersSampleAsync(int projectId);
    Task<ProjectDetailsReadDto> GetDetailsAsync(int projectId);

}
