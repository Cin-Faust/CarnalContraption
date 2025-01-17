using CarnalContraption.Domain;
using ErrorOr;

namespace CarnalContraption.Application.Storage;

public interface IEntityRepository<TEntity, in TId> where TEntity : Entity<TId> where TId : IEquatable<TId>
{
    Task<ErrorOr<Success>> CreateAsync(TEntity entity);
    Task<ErrorOr<TEntity>> RetrieveByIdAsync(TId id);
    Task<ErrorOr<Success>> UpdateAsync(TEntity entity);
    Task<ErrorOr<Success>> DeleteAsync(TId id);
    Task<ErrorOr<IEnumerable<TEntity>>> AllAsync();
}