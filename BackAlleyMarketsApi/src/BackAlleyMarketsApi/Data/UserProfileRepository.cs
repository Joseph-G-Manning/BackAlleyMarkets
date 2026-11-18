using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace BackAlleyMarketsApi.Data;

public class UserProfileRepository : IUserProfileRepository
{
    private const int UniqueConstraintViolation = 2627;
    private const int UniqueIndexViolation = 2601;

    private static readonly string CreateUserProfileSql = LoadSql("PostUser.sql");

    private readonly string _connectionString;

    public UserProfileRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("BackAlleyMarkets")
            ?? throw new InvalidOperationException(
                "Connection string 'BackAlleyMarkets' is missing."
            );
    }

    public async Task<int> CreateUserProfileAsync(
        string firstName,
        string lastName,
        string userName,
        string passwordHash,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // If anything throws before Commit, disposing the transaction rolls it back.
            await using var transaction = (SqlTransaction)
                await connection.BeginTransactionAsync(cancellationToken);

            await using var command = new SqlCommand(CreateUserProfileSql, connection, transaction);
            command.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = firstName;
            command.Parameters.Add("@LastName", SqlDbType.VarChar, 50).Value = lastName;
            command.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = userName;
            command.Parameters.Add("@UserPassword", SqlDbType.VarChar, 255).Value = passwordHash;

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var userId = Convert.ToInt32(result);

            await transaction.CommitAsync(cancellationToken);

            return userId;
        }
        catch (SqlException ex)
            when (ex.Number is UniqueConstraintViolation or UniqueIndexViolation)
        {
            throw new DuplicateUserNameException(userName, ex);
        }
    }

    private static string LoadSql(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Matches on the file name so it works whatever the project's root namespace is.
        var resourceName =
            assembly
                .GetManifestResourceNames()
                .SingleOrDefault(name =>
                    name.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase)
                )
            ?? throw new InvalidOperationException(
                $"Embedded SQL resource '{fileName}' was not found. Check that its Build Action is EmbeddedResource."
            );

        using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Could not open embedded SQL resource '{resourceName}'."
            );
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
