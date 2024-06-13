
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;
    
[Table("UsersSkills")]
public class UserSkill
{
    public int Id { get; set; }
    public required User User { get; set; }
    public required Skill Skill { get; set;}

}
