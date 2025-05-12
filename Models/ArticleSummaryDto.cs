public class ArticleSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime? SubmittedDate { get; set; }
}
