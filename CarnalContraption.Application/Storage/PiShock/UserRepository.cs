using System.Data;
using CarnalContraption.Domain.PiShock;
using CarnalContraption.Domain.Users;
using Dapper;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CarnalContraption.Application.Storage.PiShock;

internal class UserRepository(ILogger<UserRepository> logger, IDbConnection dbConnection) : IUserRepository
{
    private const int None = 0;
    private const string TableName = "pishockusers";
    private const string SqlGetExistingIdsCount = $"SELECT COUNT(*) FROM {TableName} WHERE Id = @Id";
    private const string SqlGetExistingNamesCount = $"SELECT COUNT(*) FROM {TableName} WHERE Name = @Name";
    private const string SqlInsert = $"INSERT INTO {TableName} VALUES (@Id, @Name, @ApiKey, @Code)";
    private const string SqlSelectById = $"SELECT Id, Name, ApiKey, ShareCode FROM {TableName} WHERE Id = @Id LIMIT 1";

    private static class Errors
    {
        public static readonly Error Exception = Error.Unexpected("PiShock.UserRepository.Exception");
        public static readonly Error AlreadyExists = Error.Conflict("PiShock.UserRespository.AlreadyExists", "User already exists.");
        public static readonly Error UsernameAlreadyExists = Error.Conflict("PiShock.UserRespository.UsernameAlreadyExists", "Username already exists.");
        public static readonly Error NotFound = Error.NotFound("PiShock.UserRepository.NotFound");
    }

    public async Task<ErrorOr<Success>> CreateAsync(User entity)
    {
        try
        {
            var existingIdsCount = await dbConnection.QueryFirstAsync<int>(SqlGetExistingIdsCount, new
            {
                Id = entity.Id.Value
            });

            if (existingIdsCount > None) return Errors.AlreadyExists;

            var existingNamesCount = await dbConnection.QueryFirstAsync<int>(SqlGetExistingNamesCount, new
            {
                Name = entity.Name.Value
            });

            if (existingNamesCount > None) return Errors.UsernameAlreadyExists;

            await dbConnection.ExecuteAsync(SqlInsert, new DbUser
                {
                    Id = entity.Id.Value,
                    Name = entity.Name.Value,
                    ApiKey = entity.ApiKey.Value,
                    ShareCode = entity.ShareCode.Value
                }
            );

            return new Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Errors.Exception;
        }
    }

    public async Task<ErrorOr<User>> RetrieveByIdAsync(UserId id)
    {
        try
        {
            var dbUser = await dbConnection.QueryFirstOrDefaultAsync<DbUser>(SqlSelectById, new { Id = id.Value });
            if (dbUser == null) return Errors.NotFound;

            var userIdResult = UserId.Create(dbUser.Id);
            if (userIdResult.IsError) return userIdResult.Errors;

            var usernameResult = Username.Create(dbUser.Name);
            if (usernameResult.IsError) return usernameResult.Errors;

            var apiKeyResult = ApiKey.Create(dbUser.ApiKey);
            if (apiKeyResult.IsError) return apiKeyResult.Errors;

            var codeResult = ShareCode.Create(dbUser.ShareCode);
            if (codeResult.IsError) return codeResult.Errors;
            return new User(userIdResult.Value, usernameResult.Value, apiKeyResult.Value, codeResult.Value);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Errors.Exception;
        }
    }

    public Task<ErrorOr<Success>> UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Success>> DeleteAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<IEnumerable<User>>> AllAsync()
    {
        throw new NotImplementedException();
    }

    private record DbUser
    {
        public ulong Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string ApiKey { get; init; } = string.Empty;
        public string ShareCode { get; init; } = string.Empty;
    }
}