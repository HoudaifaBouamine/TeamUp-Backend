using Microsoft.AspNetCore.Mvc;
using Repositories;
using Asp.Versioning;
using Utils;

namespace Controllers;

[Tags("Skills Group")]
[ApiVersion(1)]
[ApiController]
[Route("api/v{version:apiVersion}/skills")]
public class SkillController(IUserRepository userRepository, ISkillRepository skillRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISkillRepository _skillRepository = skillRepository;

    [HttpPost("{userId}/{skillId}")]
    public async Task<IActionResult> AddSkillToUser(string userId, int skillId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found"));
        }

        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill is null)
        {
            return NotFound(new ErrorResponse("Skill not found"));
        }

        user.Skills.Add(skill);

        await _userRepository.UpdateAsync(user);
        return Ok();
    }

    [HttpDelete("{userId}/{skillId}")]
    public async Task<IActionResult> RemoveSkillFromUser(string userId, int skillId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found"));
        }

        var skill = user.UserSkills.FirstOrDefault(s => s.Id == skillId);
        if (skill is null)
        {
            return NotFound(new ErrorResponse("Skill not found for the user"));
        }

        user.UserSkills.Remove(skill);

        await _userRepository.UpdateAsync(user);
        return NoContent();
    }
}

