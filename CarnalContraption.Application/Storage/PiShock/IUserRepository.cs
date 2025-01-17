using CarnalContraption.Domain.PiShock;
using CarnalContraption.Domain.Users;

namespace CarnalContraption.Application.Storage.PiShock;

internal interface IUserRepository : IEntityRepository<User, UserId>;