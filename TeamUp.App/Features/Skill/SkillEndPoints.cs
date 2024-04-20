using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories;
using Users; 
namespace Controllers
{    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISkillRepository _skillRepository;

        public SkillController(IUserRepository userRepository, ISkillRepository skillRepository)
        {
            _userRepository = userRepository;
            _skillRepository = skillRepository;
        }


        [HttpPost("{userId}/skills/{skillId}")]
        public async Task<ActionResult> AddSkillToUser(string userId, int skillId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var skill = await _skillRepository.GetByIdAsync(skillId);
            if (skill == null)
            {
                return NotFound("Skill not found");
            }

            //user.UserSkills.Add(skill);

            await _userRepository.UpdateAsync(user);
            return Ok();
        }

        [HttpDelete("{userId}/skills/{skillId}")]
        public async Task<ActionResult> RemoveSkillFromUser(string userId, int skillId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var skill = user.UserSkills.FirstOrDefault(s => s.Id == skillId);
            if (skill == null)
            {
                return NotFound("Skill not found for the user");
            }

            user.UserSkills.Remove(skill);

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
