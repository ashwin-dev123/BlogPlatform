using BlogPlatform.Application.DTOs;
using BlogPlatform.Application.Interfaces;
using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly BlogDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(BlogDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email); // fetch user with some email

        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;  // if password is not correct, return null

        var token = GenerateJwtToken(user); // generate JWT token for the user

        return new AuthResponseDto  // return the token and user info
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<UserDto?> RegisterAsync(CreateUserDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);// fetch user with some email

        if (existingUser != null)
            return null; // if user with the same email already exists, return null

        // HASH PASSWORD
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);


        var user = new User(dto.Username, dto.Email, hashedPassword);// create a new user object

        _context.Users.Add(user); // add the user to the database context
        await _context.SaveChangesAsync(); // save changes to the database

        return new UserDto // return the created user info (without password)
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey( 
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}