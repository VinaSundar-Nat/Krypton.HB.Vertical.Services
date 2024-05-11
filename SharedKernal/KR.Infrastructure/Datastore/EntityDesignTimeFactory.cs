using System;
using KR.Infrastructure.Datastore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace KR.Infrastructure.Datastore;

public abstract class EntityDesignTimeFactory<T> : IDesignTimeDbContextFactory<T>
    where T : DbContext
{
    protected const string environment = "ASPNETCORE_ENVIRONMENT";
    protected  string ConnectionDetails { get; set; }

    protected IConfigurationRoot Configuration { get; set; }

    public T CreateDbContext(string[] args)
    {
        SetConfiguration();
        return Create();
    }

    T Create()
    {
        DbSettings dbSettings = new();
        Configuration.GetSection(ConnectionDetails).Bind(dbSettings);
      
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbSettings.ConnectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<T>();
        optionsBuilder.UseNpgsql(dbSettings.ConnectionString, o => o.UseNetTopologySuite());
        return CreateCustomContext(optionsBuilder.Options);
    }

    public abstract void SetConfiguration();

    public abstract T CreateCustomContext(DbContextOptions<T> options);

}


