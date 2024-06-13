

using Models;

namespace DTos ; 
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<User> Users { get; set; } = [];

    }
    public class GetSkillDto
    {
        public int ProjectsCount { get; set; }
        public int UsersCount { get; set; }
        public string Name { get; set; } = string.Empty;
    }
