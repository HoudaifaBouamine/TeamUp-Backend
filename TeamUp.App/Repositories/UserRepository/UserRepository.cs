using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Repositories;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task AddSkillAsync(string userId, Skill skill)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Skill>> GetSkillsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveSkillAsync(string userId, int skillId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}

