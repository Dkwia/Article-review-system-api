public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;  
    public DateTime? SubmittedDate { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public string FilePath { get; set; } = string.Empty; 
    
    public ICollection<ReviewRequest> ReviewRequests { get; set; } = new List<ReviewRequest>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
