using System;
using KR.Infrastructure.Datastore.Interface;
using Microsoft.EntityFrameworkCore;

namespace KR.Infrastructure.Datastore;

public class BaseUnitOfWork : IUnitOfWork
{
    protected readonly DbContext context;
    public BaseUnitOfWork(DbContext _context)
    {
        context = _context;
    }

    public async Task<bool> SaveAsyc() => await context.SaveChangesAsync() >= 0;
    public bool Save() => context.SaveChanges() >= 0;
}


