using System.ComponentModel.DataAnnotations.Schema;

namespace Models {        

[Table("UsersReviews")]
public class UserReview
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;  
    public required User ReviewerUserId { get; set; }
    public required User ReviewedUserId { get; set; }
    public byte stars { get; set; }
}



}