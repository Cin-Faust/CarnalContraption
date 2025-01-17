namespace CarnalContraption.Application;

public abstract record GuildUserRequest<TResult>(ulong UserId, ulong GuildId) : UserRequest<TResult>(UserId);