using Microsoft.AspNetCore.Mvc;
using Repositories;
using Asp.Versioning;
using Utils;
using Models;
using DTos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Controllers;


[Tags("Skills Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/skills")]
[ApiController]
public class SkillController(IUserRepository userRepository, ISkillRepository skillRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISkillRepository _skillRepository = skillRepository;

        [HttpGet]
       public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillRepository.GetAllAsync();
            var skillDtos = new List<GetSkillDto>();
            foreach (var skill in skills)
            {
                skillDtos.Add(new GetSkillDto
                {
                    Name = skill.Name
                });
            }
            return Ok(skillDtos);
        }



    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddSkillToUser([FromQuery] string skillName,[FromServices] UserManager<User> userManager, [FromServices] AppDbContext db)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found"));
        }

        var skill = await db.Skills.FirstOrDefaultAsync(s => s.Name == skillName);
        if (skill is null)
        {
            return NotFound(new ErrorResponse("Skill not found"));
        }

        user.Skills.Add(skill);

        await userManager.UpdateAsync(user);
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveSkillFromUser(
        [FromQuery] string skillName,
        [FromServices] UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found"));
        }

        var skill = user.UserSkills.FirstOrDefault(s => s.Skill.Name == skillName);
        if (skill is null)
        {
            return NotFound(new ErrorResponse("Skill not found for the current user"));
        }

        user.UserSkills.Remove(skill);

        await userManager.UpdateAsync(user);
        return NoContent();
    }
}

