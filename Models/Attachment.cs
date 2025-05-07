public class Attachment
{
    public int Id { get; set; }
    public int? ReviewId { get; set; }
    public Review Review { get; set; }
    public int? ArticleId { get; set; }
    public Article Article { get; set; }
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
