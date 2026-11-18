using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BackAlleyMarketsApi.Data;

namespace BackAlleyMarketsApi.Controllers;

public class CreateUserProfileRequest
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string UserPassword { get; set; } = string.Empty;
}

[ApiController]
[Route("api/user-profile")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(
        IUserProfileRepository userProfileRepository,
        ILogger<UserProfileController> logger
    )
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserProfile(
        [FromBody] CreateUserProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var userName = request.UserName.Trim();

        if (firstName.Length == 0 || lastName.Length == 0 || userName.Length == 0)
        {
            return BadRequest("First name, last name, and username cannot be blank.");
        }

        // Store a hash, never the plain-text password.
        // Requires the BCrypt.Net-Next NuGet package.
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.UserPassword);

        try
        {
            var userId = await _userProfileRepository.CreateUserProfileAsync(
                firstName,
                lastName,
                userName,
                passwordHash,
                cancellationToken
            );

            return Created($"/api/user-profile/{userId}", new { userId });
        }
        catch (DuplicateUserNameException)
        {
            return Conflict("That username is already taken.");
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while creating a user profile.");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Unable to create the profile."
            );
        }
    }
}
