using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;  
    
[Table("Skills")]
public class Skill
{
    [Key]
    public required string Name { get; set; }

    //each skill has many users , so we collaborate it with a list of UserSkill
    
}
