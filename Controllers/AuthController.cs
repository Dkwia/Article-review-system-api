using ArticleReviewSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }
    
    [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    var user = new User
    {
        UserName = registerDto.Email,
        Email = registerDto.Email,
        FirstName = registerDto.FirstName,
        LastName = registerDto.LastName,
        Role = registerDto.Role,
        // Optional fields
        Bio = registerDto.Bio,
        Location = registerDto.Location,
        Institution = registerDto.Institution,
        SocialLinks = registerDto.SocialLinks,
        Specialization = registerDto.Specialization
    };

    var result = await _userManager.CreateAsync(user, registerDto.Password);
    
    if (!result.Succeeded)
    {
        return BadRequest(result.Errors);
    }
    
    return Ok(new { Message = "User registered successfully" });
}    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid credentials");
        }
        
        var token = GenerateJwtToken(user);
        
        return Ok(new { Token = token, User = new { user.Id, user.Email, user.Role } });
    }

 [HttpGet("profile")]
[Authorize]  // Requires authentication
public async Task<IActionResult> GetProfile()
{
    // Get current user ID from claims
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    // Get user from database
    var user = await _userManager.FindByIdAsync(userId);
    
    if (user == null)
    {
        return NotFound();
    }

    // Return profile data (create a DTO to avoid sending sensitive data)
    var profileDto = new ProfileDto
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role,
        Specialization = user.Specialization,
        Institution = user.Institution,
        Bio = user.Bio,
        Location = user.Location,
        SocialLinks = user.SocialLinks
    };

    return Ok(profileDto);
}   
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));
        
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: expires,
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RegisterDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Specialization { get; set; }
    public string Institution { get; set; }
    public string Bio { get; set; }
    public string Location { get; set; }
    public string SocialLinks { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
