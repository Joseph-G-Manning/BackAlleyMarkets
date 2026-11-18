namespace BackAlleyMarketsApi.Data;

public interface IUserProfileRepository
{
    /// <summary>
    /// Creates the User_Profile and Profile_Login rows in one transaction.
    /// Returns the new UserId.
    /// </summary>
    /// <exception cref="DuplicateUserNameException">The username already exists.</exception>
    Task<int> CreateUserProfileAsync(
        string firstName,
        string lastName,
        string userName,
        string passwordHash,
        CancellationToken cancellationToken = default
    );
}

public class DuplicateUserNameException : Exception
{
    public DuplicateUserNameException(string userName, Exception? innerException = null)
        : base($"The username '{userName}' is already taken.", innerException) { }
}
