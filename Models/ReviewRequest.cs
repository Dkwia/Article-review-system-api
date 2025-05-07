public class ReviewRequest
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public int? ReviewerId { get; set; }
    public User Reviewer { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public string ExpectedTime { get; set; }
    public int? Pages { get; set; }
}
