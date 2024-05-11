using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MediatR;
using KR.Infrastructure.Datastore.Events;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace KR.Infrastructure.Datastore;

public abstract class BaseContext<T> : DbContext where T : DbContext
{
    protected  IMediator? _mediatr { get; set; }

    public BaseContext(DbContextOptions<T> options)
        : base(options)
    {

    }

    private void notify(CancellationToken token = default(CancellationToken)) => ChangeTracker.Entries().Audit((before, after) =>
    {
        if(_mediatr == null)
            _mediatr = this.GetService<IMediator>();

        _mediatr?.Publish(new ChangeNotification(before, after), token);
    });

    public override int SaveChanges()
    {
        notify();
        return base.SaveChanges();
    }

    public async override Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken))
    {
        notify(token);
        return await base.SaveChangesAsync(token);
    }
}


