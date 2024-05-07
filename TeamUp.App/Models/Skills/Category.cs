using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("Categories")]
public class Category
{
    [Key]
    public required string Name { get; set; }
}