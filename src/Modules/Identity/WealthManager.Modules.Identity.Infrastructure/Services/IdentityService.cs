using Microsoft.AspNetCore.Identity;
using WealthManager.Modules.Identity.Application.Interfaces;
using WealthManager.Modules.Identity.Application.DTOs;
using WealthManager.Modules.Identity.Domain.Entities;
using WealthManager.Modules.Identity.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using WealthManager.Modules.Identity.Domain.Constants;

using Microsoft.AspNetCore.Http;
using WealthManager.Shared.Exceptions;


namespace WealthManager.Modules.Identity.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly WealthManagerIdentityDbContext _context;
    private readonly ITokenService _tokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, ITokenService tokenService, WealthManagerIdentityDbContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;

    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Failed to create user: {errors}");
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        return user.Id;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            // Security: Use generic message to prevent user enumeration attacks
            throw new UnauthorizedException("Invalid credentials");
        }

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isValid)
        {
            // Security: Same generic message to prevent revealing which part failed
            throw new UnauthorizedException("Invalid credentials");
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        // Note: TokenService should never return null/empty, but defensive check
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InternalServerException("Failed to generate access token");
        }

        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        if (refreshToken == null)
        {
            throw new InternalServerException("Failed to generate refresh token");
        }

        _context.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthenticationResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            IsVerified = user.EmailConfirmed,
            JwToken = accessToken,
            RefreshToken = refreshToken
        };

        return response;
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        //1. find token in DB
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        //2. check if token is expired
        if (storedToken.Expires < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired");
        }

        //3. check if token is revoked
        if (storedToken.Revoked != null)
        {
            throw new UnauthorizedException("Refresh token has been revoked");
        }

        //4. validate token ownership
        if (storedToken.UserId != request.UserId)
        {
            throw new UnauthorizedException("Invalid token owner");
        }

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);

        if (string.IsNullOrEmpty(newAccessToken))
        {
            throw new InternalServerException("Failed to generate access token");
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        if (newRefreshToken == null)
        {
            throw new InternalServerException("Failed to generate refresh token");
        }

        storedToken.Revoked = DateTime.UtcNow;
        _context.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthenticationResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            IsVerified = user.EmailConfirmed,
            JwToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return response;
    }
}