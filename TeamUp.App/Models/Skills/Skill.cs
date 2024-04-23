using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ; 

namespace Models {        
    
[Table("Skills")]
public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }

    //each skill has many users , so we collaborate it with a list of UserSkill
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();



    }


}