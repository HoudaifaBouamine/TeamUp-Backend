using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(Name))]
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
}