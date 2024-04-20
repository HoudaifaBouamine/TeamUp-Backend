// ISkillRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public interface ISkillRepository
    {
        Task<IEnumerable<Skill>> GetAllAsync();
        Task<Skill> GetByIdAsync(int id);
        Task AddAsync(Skill skill);
        Task DeleteAsync(int id);
    }
}
