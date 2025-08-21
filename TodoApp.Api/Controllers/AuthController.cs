using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _um;
    private readonly IConfiguration _cfg;

    public AuthController(UserManager<IdentityUser> um, IConfiguration cfg)
    {
        _um = um; _cfg = cfg;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        var result = await _um.CreateAsync(user, dto.Password);

        if (result.Succeeded) return Ok();

        // Return clean list of descriptions so the client can display them
        var errors = result.Errors.Select(e => e.Description).ToArray();
        return BadRequest(new { errors });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _um.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized();

        var ok = await _um.CheckPasswordAsync(user, dto.Password);
        if (!ok) return Unauthorized();

        var token = CreateToken(user);
        return Ok(new { token });
    }

    private string CreateToken(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);