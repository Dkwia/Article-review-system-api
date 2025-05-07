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
    
    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles()
    {
        var articles = await _context.Articles
            .Include(a => a.Author)
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
