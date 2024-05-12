namespace DTos ; 
public class UserReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; }= "";
        public byte Stars { get; set; }
        public int ReviewerUserId { get; set; }
        public int ReviewedUserId { get; set; }
    }


 
 public class ProjectReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public byte Stars { get; set; }
        public int ReviewerUserId { get; set; }
        public int ReviewedProjectId { get; set; }
    }


