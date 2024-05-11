using System;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Microsoft.Extensions.Logging;
using KR.Infrastructure.Datastore.Interface;
using System.Threading.Tasks;

namespace KR.Infrastructure.Datastore;

public abstract class BaseRepository<TEntity, K> : BaseUnitOfWork, IRepository<TEntity>
    where TEntity : BaseEntity<TEntity, K>, IAggregateRoot
{
    protected readonly ILogger logger;
    protected DbSet<TEntity> DBset => context.Set<TEntity>();

    public BaseRepository(DbContext _context, ILogger _logger) :
    base(_context)
    {
        logger = _logger;
    }

    public async virtual Task<bool> Create(TEntity entity, bool isDependent = false)
    {
        var tracker = DBset.Add(entity as TEntity);
        tracker.State = EntityState.Added;

        if (isDependent)
            return true;

        var response = await context.SaveChangesAsync();
        return !(response <= 0);
    }

    public async virtual Task<bool> Update(TEntity entity, bool isDependent = false)
    {
            var tracker = DBset.Update(entity as TEntity);
            tracker.State = EntityState.Modified;

            if (isDependent)
                return true;

            var response = await context.SaveChangesAsync();
            return response <= 0 ? false : true;     
    }

    public virtual bool Delete(TEntity entity, bool isDependent = false)
    {
        var tracker = DBset.Remove(entity as TEntity);
        tracker.State = EntityState.Deleted;

        if (isDependent)
            return true;

        var response = context.SaveChanges();
        return response <= 0 ? false : true;
    }

    public async virtual Task BulkUpsertAsync(IList<TEntity> inserts, IList<TEntity> updates,
            IList<TEntity> deletes )
    {
        using (var transaction = new CommittableTransaction(new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        {
            context.Database.EnlistTransaction(transaction);
            context.AddRange(inserts);

            Parallel.ForEach(updates, async (entity) =>
            {
                var tracker = DBset.Update(entity);
                tracker.State = EntityState.Modified;
            });

            Parallel.ForEach(deletes, async (entity) =>
            {
                var tracker = DBset.Remove(entity);
                tracker.State = EntityState.Deleted;
            });

            var entries = await context.SaveChangesAsync();
            transaction.Commit();
        }
    }

    public async virtual Task<IList<TEntity>> BulkCreateAsync(IList<TEntity> entityCollection)
    {
        var response = new int[entityCollection.Count()];
        using (var transaction = new CommittableTransaction(new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        {
            context.Database.EnlistTransaction(transaction);
            context.AddRange(entityCollection);
            var inserted = await context.SaveChangesAsync();

            if (inserted != entityCollection.Count())
                throw new DbUpdateException($"Error performing bulk save - {nameof(TEntity)}");

            transaction.Commit();
        }

        return entityCollection;
    }

    public async virtual Task<bool> BulkDeleteAsync(IList<TEntity> entityCollection)
    {
        var tasks = new List<int>();

        using (var transaction = new CommittableTransaction(new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        {
            context.Database.EnlistTransaction(transaction);

            Parallel.ForEach(entityCollection ,async (IAggregateRoot entity) =>
            {
                var _entity = DBset.Remove(entity as TEntity);
                _entity.State = EntityState.Deleted;
                 tasks.Add(await context.SaveChangesAsync() );
            });

            if (tasks.Any(a => a <= 0))
                throw new DbUpdateException($"Error performing bulk delete - {nameof(TEntity)}");

            transaction.Commit();
        }

        return true;
    }

}

