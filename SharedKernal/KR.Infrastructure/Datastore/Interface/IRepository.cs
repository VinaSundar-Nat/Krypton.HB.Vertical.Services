using System;
namespace KR.Infrastructure.Datastore.Interface;

public interface IRepository<TEntity> : IUnitOfWork
{
    Task<bool> Create(TEntity entity, bool isDependent);
    Task<IList<TEntity>> BulkCreateAsync(IList<TEntity> entityCollection);
    Task<bool> Update(TEntity entity, bool isDependent);
    bool Delete(TEntity entity, bool isDependent);
    Task<bool> BulkDeleteAsync(IList<TEntity> entityCollection);
    Task BulkUpsertAsync(IList<TEntity> inserts, IList<TEntity> updates,
           IList<TEntity> deletes);
}


