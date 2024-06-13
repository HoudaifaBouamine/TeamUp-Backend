// ISkillRepository.cs
using Models;

namespace Repositories
{
    public interface ISkillRepository
    {
        Task<IEnumerable<Skill>> GetAllAsync();
        Task AddAsync(Skill skill);
        Task DeleteAsync(int id);
    }
}
