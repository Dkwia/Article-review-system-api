using ArticleReviewSystem.Models;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Reviewer,Admin")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var reviews = await _context.Reviews
            .Include(r => r.Article)
            .ThenInclude(a => a.Author)
            .Where(r => r.ReviewerId == int.Parse(userId))
            .Where(r => r.Status == "Submitted")
            .ToListAsync();
            
        return Ok(reviews);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var review = new Review
        {
            ArticleId = reviewDto.ArticleId,
            ReviewerId = int.Parse(userId),
            Rating = reviewDto.Rating,
            Recommendation = reviewDto.Recommendation,
            TechnicalMerit = reviewDto.TechnicalMerit,
            Originality = reviewDto.Originality,
            PresentationQuality = reviewDto.PresentationQuality,
            CommentsToAuthors = reviewDto.CommentsToAuthors,
            ConfidentialComments = reviewDto.ConfidentialComments,
            Status = "Draft"
        };
        
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }
    
    [HttpPut("{id}/submit")]
    public async Task<IActionResult> SubmitReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        
        if (review == null)
        {
            return NotFound();
        }
        
        review.Status = "Submitted";
        review.SubmittedAt = DateTime.UtcNow;
        
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        
        var article = await _context.Articles.FindAsync(review.ArticleId);
        if (article != null)
        {
            article.Status = review.Recommendation switch
            {
                "Accept" => "Accepted",
                "AcceptWithMinorRevisions" => "NeedsRevision",
                "AcceptWithMajorRevisions" => "NeedsRevision",
                "Reject" => "Rejected",
                _ => article.Status
            };
            
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }
        
        return Ok(review);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Article)
            .Include(r => r.Reviewer)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (review == null)
        {
            return NotFound();
        }
        
        return Ok(review);
    }
}

public class ReviewDto
{
    public int ArticleId { get; set; }
    public int? Rating { get; set; }
    public string Recommendation { get; set; }
    public string TechnicalMerit { get; set; }
    public string Originality { get; set; }
    public string PresentationQuality { get; set; }
    public string CommentsToAuthors { get; set; }
    public string ConfidentialComments { get; set; }
}
