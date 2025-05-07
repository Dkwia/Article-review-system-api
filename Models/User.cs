using Microsoft.AspNetCore.Identity;
public class User : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; } 
    public string Specialization { get; set; }
    public string Institution { get; set; }
    public string Bio { get; set; }
    public string Location { get; set; }
    public string SocialLinks { get; set; } 
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Article> AuthoredArticles { get; set; }
    public ICollection<ReviewRequest> ReviewRequests { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
