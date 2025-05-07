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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var requests = await _context.ReviewRequests
            .Include(r => r.Article)
            .ThenInclude(a => a.Author)
            .Where(r => r.ReviewerId == null || r.ReviewerId == int.Parse(userId))
            .Where(r => r.Status == "Pending")
            .ToListAsync();
            
        return Ok(requests);
    }
    
    [HttpGet("inprogress")]
    public async Task<IActionResult> GetInProgressReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var requests = await _context.ReviewRequests
            .Include(r => r.Article)
            .ThenInclude(a => a.Author)
            .Where(r => r.ReviewerId == int.Parse(userId))
            .Where(r => r.Status == "Accepted")
            .ToListAsync();
            
        return Ok(requests);
    }
    
    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptReviewRequest(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var request = await _context.ReviewRequests.FindAsync(id);
        
        if (request == null)
        {
            return NotFound();
        }
        
        request.ReviewerId = int.Parse(userId);
        request.Status = "Accepted";
        
        _context.ReviewRequests.Update(request);
        await _context.SaveChangesAsync();
        
        return Ok(request);
    }
    
    [HttpPut("{id}/decline")]
    public async Task<IActionResult> DeclineReviewRequest(int id)
    {
        var request = await _context.ReviewRequests.FindAsync(id);
        
        if (request == null)
        {
            return NotFound();
        }
        
        request.Status = "Declined";
        
        _context.ReviewRequests.Update(request);
        await _context.SaveChangesAsync();
        
        return Ok(request);
    }
}
