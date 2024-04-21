using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Repositories;
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(string id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);

    Task<IEnumerable<Skill>> GetSkillsAsync(string userId);
    Task AddSkillAsync(string userId, Skill skill);
    Task RemoveSkillAsync(string userId, int skillId);
}

