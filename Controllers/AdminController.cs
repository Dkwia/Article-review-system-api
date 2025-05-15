using ArticleReviewSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public AdminController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }
    
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto userDto)
    {
        var user = new User
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Role = userDto.Role,
            Specialization = userDto.Specialization,
            Institution = userDto.Institution
        };
        
        var result = await _userManager.CreateAsync(user, userDto.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        await _userManager.AddToRoleAsync(user, userDto.Role);
        
        return Ok(user);
    }

[HttpDelete("users/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        return NotFound(new { error = "User not found." });
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "User deleted successfully." });
}
    
    [HttpPut("users/{id}/block")]
    public async Task<IActionResult> BlockUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        
        if (user == null)
        {
            return NotFound();
        }
        
        user.Status = "Blocked";
        await _userManager.UpdateAsync(user);
        
        return Ok(user);
    }



[HttpDelete("reviews/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteReview(int id)
{
    var review = await _context.Reviews.FindAsync(id);
    if (review == null)
    {
        return NotFound(new { error = "Review not found." });
    }

    var article = await _context.Articles.FindAsync(review.ArticleId);
    if (article != null)
    {
        article.Status = "Pending";
        _context.Articles.Update(article);
    }

    _context.Reviews.Remove(review);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Review deleted successfully." });
}
    
    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles()
    {
        var articles = await _context.Articles
            .ToListAsync();
            
        return Ok(articles);
    }
    
    [HttpPost("reviewrequests")]
    public async Task<IActionResult> CreateReviewRequest([FromBody] AdminReviewRequestDto requestDto)
    {
        var request = new ReviewRequest
        {
            ArticleId = requestDto.ArticleId,
            ReviewerId = requestDto.ReviewerId,
            DueDate = requestDto.DueDate,
            ExpectedTime = requestDto.ExpectedTime,
            Pages = requestDto.Pages
        };
        
        _context.ReviewRequests.Add(request);
        await _context.SaveChangesAsync();
        
        return Ok(request);
    }

[HttpGet("reviews/completed")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetCompletedReviews()
{
    var reviews = await _context.Reviews
        .Select(r => new
        {
            r.Id,
            r.ArticleId,
            r.ReviewerId,
            r.Rating,
            r.Recommendation,
            r.TechnicalMerit,
            r.Originality,
            r.PresentationQuality,
            r.CommentsToAuthors,
            r.ConfidentialComments,
            r.Status,
            r.CreatedAt
        })
        .ToListAsync();

    return Ok(reviews);
}

[HttpDelete("article/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteArticle(int id)
{
    var article = await _context.Articles.FindAsync(id);
    if (article == null)
    {
        return NotFound(new { error = "Article not found." });
    }

    _context.Articles.Remove(article);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Article deleted successfully." });
}

}



public class AdminCreateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Specialization { get; set; }
    public string Institution { get; set; }
}

public class AdminReviewRequestDto
{
    public int ArticleId { get; set; }
    public int? ReviewerId { get; set; }
    public DateTime? DueDate { get; set; }
    public string ExpectedTime { get; set; }
    public int? Pages { get; set; }
}
