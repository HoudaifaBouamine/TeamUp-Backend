using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    AppDbContext db = db;
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

    public Task<User?> GetByIdAsync(Guid id)
    {
        return db.Users.FirstOrDefaultAsync(u=>u.Id == id);
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

