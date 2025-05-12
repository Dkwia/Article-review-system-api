using ArticleReviewSystem.Models;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Reviewer,Admin")]
public class ReviewRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewRequestsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("new")]
    public async Task<IActionResult> GetNewReviewRequests()
    {
        var pendingArticles = await _context.Articles
            .Where(a => a.Status == "Pending" || a.Status == "Draft")
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Category,
                a.SubmittedDate,
                a.AuthorId
            })
            .ToListAsync();

        return Ok(pendingArticles);
    }

    [HttpGet("pending-articles")]
[Authorize(Roles = "Reviewer,Admin")] 
public async Task<IActionResult> GetPendingArticles()
{
    var pendingArticles = await _context.Articles
        .Where(a => a.Status == "Pending" || a.Status == "Draft")
        .Select(a => new 
        {
            a.Id,
            a.Title,
            a.Category,
            a.SubmittedDate,
            a.AuthorId 
        })
        .ToListAsync();

    return Ok(pendingArticles);
}    

    [HttpGet("inprogress")]
    public async Task<IActionResult> GetInProgressReviews()
    {
        var inProgressArticles = await _context.Articles
            .Where(a => a.Status == "Accepted")
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Category,
                a.SubmittedDate,
                a.AuthorId
            })
            .ToListAsync();

        return Ok(inProgressArticles);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptReviewRequest(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound(new { error = "Article not found." });
        }

        article.Status = "Accepted";
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Article accepted for review successfully.", articleId = article.Id });
    }

    [HttpPut("{id}/decline")]
    public async Task<IActionResult> DeclineReviewRequest(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound(new { error = "Article not found." });
        }

        article.Status = "Declined";
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Article declined successfully.", articleId = article.Id });
    }
}
