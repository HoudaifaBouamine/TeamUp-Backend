using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Features.Projects; 

namespace Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectReadDto> GetByIdAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            return MapProjectToProjectReadDto(project);
        }




        public async Task<IEnumerable<ProjectReadDto>> GetAllAsync()
        {
            var projects = await _context.Projects
                .Select(p => MapProjectToProjectReadDto(p))
                .ToListAsync();
            return projects;
        }





        public async Task<int> CreateAsync(ProjectCreateDto projectDto)
        {
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                StartDate = projectDto.StartDate
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project.Id;
        }




        public async Task UpdateAsync(int id, ProjectCreateDto projectDto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                project.Name = projectDto.Name;
                project.Description = projectDto.Description;
                project.StartDate = projectDto.StartDate;

                await _context.SaveChangesAsync();
            }
        }





        public async Task DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }





        public async Task<int> GetUsersCountAsync(int projectId)
        {
            return await _context.ProjectsUsers
                .Where(pu => pu.ProjectId == projectId)
                .CountAsync();
        }





        public async Task<IEnumerable<ProjecUserShortDto>> GetUsersSampleAsync(int projectId)
        {
            var users = await _context.ProjectsUsers
                .Where(pu => pu.ProjectId == projectId)
                .Take(3)
                .Select(pu => new ProjecUserShortDto
                {
                    Id = pu.UserId,
                    ProfilePicture = pu.User.ProfilePicture
                })
                .ToListAsync();

            return users;
        }






        public async Task<ProjectDetailsReadDto> GetDetailsAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Users)
                .Include(p => p.ProjectsUsers)
                .Where(p => p.Id == projectId)
                .Select(p => new ProjectDetailsReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    UsersCount = p.Users.Count(),
                    Users = p.Users.Select(u => new ProjecUserLongDto
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName,
                        Handler = u.Handler,
                        ProfilePicture = u.ProfilePicture,
                        IsMentor = p.ProjectsUsers.FirstOrDefault(up => up.UserId == u.Id)?.IsMentor ?? false
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return project;
        }





        private ProjectReadDto MapProjectToProjectReadDto(Project project)
        {
            if (project == null)
                return null;

            return new ProjectReadDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                UsersCount = project.Users.Count(),
                UsersSample = project.Users
                    .Take(3)
                    .Select(u => new ProjecUserShortDto
                    {
                        Id = u.Id,
                        ProfilePicture = u.ProfilePicture
                    })
                    .ToList()
            };
        }
    }



}
