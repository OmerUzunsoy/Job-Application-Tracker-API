using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
