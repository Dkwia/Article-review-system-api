using ArticleReviewSystem.Models;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public ArticlesController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> GetMyArticles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var articles = await _context.Articles
            .Where(a => a.AuthorId == int.Parse(userId))
            .ToListAsync();
            
        return Ok(articles);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
            
        if (article == null)
        {
            return NotFound();
        }
        
        return Ok(article);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDto articleDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var article = new Article
        {
            Title = articleDto.Title,
            Content = articleDto.Content,
            Category = articleDto.Category,
            AuthorId = int.Parse(userId),
            Tags = articleDto.Tags,
            Status = "Draft"
        };
        
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }
    
    [HttpPut("{id}/submit")]
    public async Task<IActionResult> SubmitArticle(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id && a.AuthorId == int.Parse(userId));
            
        if (article == null)
        {
            return NotFound();
        }
        
        article.Status = "Pending";
        article.SubmittedDate = DateTime.UtcNow;
        
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();
        
        return Ok(article);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id && a.AuthorId == int.Parse(userId));
            
        if (article == null)
        {
            return NotFound();
        }
        
        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}

public class ArticleDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; }
}
