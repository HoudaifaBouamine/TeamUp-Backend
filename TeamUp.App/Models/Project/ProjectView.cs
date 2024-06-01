using System.ComponentModel.DataAnnotations.Schema;

namespace Models;


 [Table("ProjectReviews")]
    public class ProjectReview
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public byte Stars { get; set; }
        public int ReviewerUserId { get; set; }
        public User ReviewerUser { get; set; }
        public int ReviewedProjectId { get; set; }
        public Project ReviewedProject { get; set; }
    

    }
