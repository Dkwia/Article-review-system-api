public class Review
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public int ReviewerId { get; set; }
    public User Reviewer { get; set; }
    public int? Rating { get; set; }
    public string Recommendation { get; set; }
    public string TechnicalMerit { get; set; }
    public string Originality { get; set; }
    public string PresentationQuality { get; set; }
    public string CommentsToAuthors { get; set; }
    public string ConfidentialComments { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    
    public ICollection<Attachment> Attachments { get; set; }
}
