using System;
namespace KR.Infrastructure.Datastore.Interface;

public interface IUnitOfWork
{
    Task<bool> SaveAsyc();
    bool Save();
}

