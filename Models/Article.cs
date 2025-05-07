public class Article
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Category { get; set; }
    public string Status { get; set; } = "Draft";
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string FeaturedImageUrl { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<ReviewRequest> ReviewRequests { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Attachment> Attachments { get; set; }
}
