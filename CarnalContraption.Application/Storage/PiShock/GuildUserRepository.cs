using CarnalContraption.Domain.Guilds;
using CarnalContraption.Domain.Users;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;

namespace CarnalContraption.Application.Storage.PiShock;

internal class GuildUserRepository(ILogger<GuildUserRepository> logger, IDbConnection dbConnection) : IGuildUserRepository
{
    private const int None = 0;
    private const string TableName = "pishockguildusers";

    private static class Errors
    {
        public static readonly Error Exception = Error.Unexpected("PiShock.GuildUserRepository.Exception");
    }

    public async Task<ErrorOr<Success>> AddUserToGuildAsync(UserId userId, GuildId guildId)
    {
        try
        {
            var count = await dbConnection.QueryFirstAsync<int>($"SELECT COUNT(*) FROM {TableName} WHERE UserId = @UserId AND GuildId = @GuildId", new { UserId = userId.Value, GuildId = guildId.Value });
            if (count > None) return new Success();

            await dbConnection.ExecuteAsync($"INSERT INTO {TableName} VALUES (@GuildId, @UserId)", new { UserId = userId.Value, GuildId = guildId.Value });
            return new Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Errors.Exception;
        }
    }

    public async Task<ErrorOr<Success>> RemoveUserFromGuildAsync(UserId userId, GuildId guildId)
    {
        try
        {
            await dbConnection.ExecuteAsync($"DELETE FROM {TableName} WHERE GuildId = @GuildId AND UserId = @UserId LIMIT 1", new { UserId = userId.Value, GuildId = guildId.Value });
            return new Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Errors.Exception;
        }
    }

    public async Task<ErrorOr<bool>> IsUserInGuildAsync(UserId userId, GuildId guildId)
    {
        try
        {
            var count = await dbConnection.QueryFirstAsync<int>($"SELECT COUNT(*) FROM {TableName} WHERE UserId = @UserId AND GuildId = @GuildId", new { UserId = userId.Value, GuildId = guildId.Value });
            return count > None;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Errors.Exception;
        }
    }
}