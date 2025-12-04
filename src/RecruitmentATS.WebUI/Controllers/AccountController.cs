using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RecruitmentATS.WebUI.Controllers;

[Route("account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password, [FromForm] string returnUrl = "/")
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return BadRequest("Email and password are required.");
        }

        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        return Redirect($"/login?error=Invalid login attempt");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] string email, [FromForm] string password, [FromForm] string confirmPassword, [FromForm] string returnUrl = "/")
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return BadRequest("Email and password are required.");
        }

        if (password != confirmPassword)
        {
            return Redirect($"/signup?error=Passwords do not match");
        }

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await _signInManager.UserManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Assign Candidate role by default
            await _signInManager.UserManager.AddToRoleAsync(user, "Candidate");
            
            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: true);
            
            return LocalRedirect(returnUrl);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Redirect($"/signup?error={System.Net.WebUtility.UrlEncode(errors)}");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromForm] string returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect(returnUrl);
    }
}
