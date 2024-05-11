using System;
using KR.Common.Gaurd;
using MediatR;

namespace KR.Infrastructure.Datastore;

public abstract class Notification
{
    private IList<INotification>? domainEvents;
    public IReadOnlyCollection<INotification>? DomainEvents
        => domainEvents?.AsReadOnly();

    public void AddEvent(INotification domainEve)
    {
        domainEvents = domainEvents ?? new List<INotification>();
        domainEvents.Add(domainEve);
    }

    public void ClearEvents() => domainEvents?.Clear();

    public void RemoveEvent(INotification domainEve)
        => domainEvents?.Remove(domainEve);   
}

public abstract class BaseEntity<TEntity, K> : Notification, IEquatable<TEntity> 
{
   
    public K Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public uint VersionStamp { get; set; }

    public BaseEntity()
    {      
    }

    public override bool Equals(object? other)
    {
        if (other is not TEntity _compare)
            return false;

        if (other == null) return this == null;

        return this.Equals(_compare);
    }

    public virtual bool Equals(TEntity? other)
    {
        if (other is not BaseEntity<TEntity,K> _compare)
            return false;

        if (_compare == null) return this == null;

        return _compare.Id.Equals(this.Id);
    }

    public override int GetHashCode() =>
         HashCode.Combine(Id?.GetHashCode());
}

public abstract class BaseAuditEntity<TEntity, K> : BaseEntity<TEntity, K> 
{
    public DateTime? ModifieddAt { get; set; }
    public string ModifiedBy { get; set; }
}


