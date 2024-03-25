using Models;
using System.ComponentModel.DataAnnotations;

namespace TeamUp_Backend.Features.Project
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public required ChatRoom ChatRoomId { get; set; }
    }

}
