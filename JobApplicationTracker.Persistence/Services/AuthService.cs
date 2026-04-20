using JobApplicationTracker.Application.Auth;
using JobApplicationTracker.Application.Common.Exceptions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Persistence.Services;

public class AuthService(AppDbContext dbContext, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (exists)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        PasswordHasher.CreateHash(request.Password, out var hash, out var salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.FullName, user.Email, tokenService.GenerateToken(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null || !PasswordHasher.VerifyHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new UnauthorizedException("Email or password is incorrect.");
        }

        return new AuthResponse(user.Id, user.FullName, user.Email, tokenService.GenerateToken(user));
    }
}
