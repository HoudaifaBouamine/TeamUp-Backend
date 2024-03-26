using Models;
using System.ComponentModel.DataAnnotations;

namespace TeamUp_Backend.Features.Project
{
    public class ProjectCreateDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartDateTime { get; set; }
    }

    public class ProjectReadDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartDateTime { get; set; }
        public DateOnly? EndDateTime { get; set; } = null;
    }

}
