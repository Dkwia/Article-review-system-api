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
    if (!int.TryParse(userId, out int reviewerId))
    {
        return BadRequest(new { error = "Invalid user ID." });
    }

    var reviews = await _context.Reviews
        .Where(r => r.ReviewerId == reviewerId)
        .Join(
            _context.Articles,
            review => review.ArticleId,
            article => article.Id,
            (review, article) => new
            {
                Review = review,
                ArticleTitle = article.Title,
                AuthorId = article.AuthorId
            }
        )
        .Join(
            _context.Users,
            combined => combined.AuthorId,
            user => user.Id,
            (combined, user) => new CompletedReviewResponseDto
            {
                Id = combined.Review.Id,
                ArticleId = combined.Review.ArticleId,
                ArticleTitle = combined.ArticleTitle,
                AuthorName = user.UserName, 
                Rating = combined.Review.Rating,
                Recommendation = combined.Review.Recommendation,
                Status = combined.Review.Status,
                CreatedAt = combined.Review.CreatedAt
            }
        )
        .ToListAsync();

    return Ok(reviews);
}
    
    [HttpPost]
public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
{

    if (reviewDto == null)
    {
        return BadRequest(new { error = "Invalid request body." });
    }

    if (reviewDto.ArticleId <= 0)
    {
        return BadRequest(new { error = "Invalid ArticleId." });
    }

    var article = await _context.Articles.FindAsync(reviewDto.ArticleId);
    if (article == null)
    {
        return NotFound(new { error = "Article not found." });
    }

    if (article.Status != "Accepted")
    {
        return BadRequest(new { error = "Article is not in a reviewable state." });
    }

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userId, out int reviewerId))
    {
        return BadRequest(new { error = "Invalid user ID." });
    }

    var review = new Review
    {
        ArticleId = reviewDto.ArticleId,
        ReviewerId = reviewerId,
        Rating = reviewDto.Rating,
        Recommendation = reviewDto.Recommendation,
        TechnicalMerit = reviewDto.TechnicalMerit,
        Originality = reviewDto.Originality,
        PresentationQuality = reviewDto.PresentationQuality,
        CommentsToAuthors = reviewDto.CommentsToAuthors,
        ConfidentialComments = reviewDto.ConfidentialComments,
        Status = "Draft",
        CreatedAt = DateTime.UtcNow
    };

    _context.Reviews.Add(review);

    article.Status = "Submitted";
    _context.Articles.Update(article);

    await _context.SaveChangesAsync();

    var response = new ReviewResponseDto
    {
        Id = review.Id,
        ArticleId = review.ArticleId,
        ReviewerId = review.ReviewerId,
        Rating = review.Rating,
        Recommendation = review.Recommendation,
        TechnicalMerit = review.TechnicalMerit,
        Originality = review.Originality,
        PresentationQuality = review.PresentationQuality,
        CommentsToAuthors = review.CommentsToAuthors,
        ConfidentialComments = review.ConfidentialComments,
        Status = review.Status,
        CreatedAt = review.CreatedAt
    };

    return CreatedAtAction(nameof(GetReview), new { id = review.Id }, response);
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
        .Select(r => new ReviewResponseDto
        {
            Id = r.Id,
            ArticleId = r.ArticleId,
            ReviewerId = r.ReviewerId,
            Rating = r.Rating,
            Recommendation = r.Recommendation,
            TechnicalMerit = r.TechnicalMerit,
            Originality = r.Originality,
            PresentationQuality = r.PresentationQuality,
            CommentsToAuthors = r.CommentsToAuthors,
            ConfidentialComments = r.ConfidentialComments,
            Status = r.Status,
            CreatedAt = r.CreatedAt
        })
        .FirstOrDefaultAsync(r => r.Id == id);

    if (review == null)
    {
        return NotFound(new { error = "Review not found." });
    }

    var reviewer = await _context.Users
        .Where(u => u.Id == review.ReviewerId)
        .Select(u => new { u.UserName, u.Email })
        .FirstOrDefaultAsync();

    

    return Ok(review);
}
}

public class ReviewDto
{
    public int ArticleId { get; set; }
    public int? Rating { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string TechnicalMerit { get; set; } = string.Empty;
    public string Originality { get; set; } = string.Empty;
    public string PresentationQuality { get; set; } = string.Empty;
    public string CommentsToAuthors { get; set; } = string.Empty;
    public string ConfidentialComments { get; set; } = string.Empty;
}

public class ReviewResponseDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int ReviewerId { get; set; }
    public int? Rating { get; set; }
    public string Recommendation { get; set; }
    public string TechnicalMerit { get; set; }
    public string Originality { get; set; }
    public string PresentationQuality { get; set; }
    public string CommentsToAuthors { get; set; }
    public string ConfidentialComments { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CompletedReviewResponseDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
