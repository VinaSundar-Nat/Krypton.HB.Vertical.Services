using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using KR.Infrastructure.Datastore.Interface;
using System.Reflection;
using KR.Common.Attributes.Persistance;
using Npgsql;
using KR.Infrastructure.Datastore.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyModel;
using MediatR;

namespace KR.Infrastructure.Datastore;

public static class DBExtensions
{
    public static void Audit(this IEnumerable<EntityEntry> entries, Action<PropertyValues, PropertyValues> _actionAudit)
    {
        foreach (var entry in entries
            .Where(e => e.State != EntityState.Unchanged || e.State != EntityState.Detached))
        {
            if (!typeof(IAuditiable).IsAssignableFrom(entry.Entity.GetType()))
                return;

            var current = entry.CurrentValues;
            var before = entry.GetDatabaseValues();

            _actionAudit?.Invoke(before, current);
        }
    }

    public static void ApplyAllConfigurations(this ModelBuilder modelBuilder, Type requestor)
    {
        var applyConfigurationMethodInfo = modelBuilder
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(m => m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));

        var associatedTypes = requestor.Assembly.GetTypes()
                                .Where(t => t.GetCustomAttributes(typeof(DBConfigurationAttribute), true).Any());

        var instances = associatedTypes
                        .Select(i =>
                        (
                            mpt: i.GetInterfaces()?.FirstOrDefault(a => a.GetTypeInfo().IsGenericType == true)
                                        ?.GetGenericArguments()[0],
                            mptO: Activator.CreateInstance(i)
                        )).ToList();


        instances.ForEach(it =>
        {
            var concRegister = applyConfigurationMethodInfo.MakeGenericMethod(it.mpt);
            concRegister.Invoke(modelBuilder, new[] { it.mptO });
        });
    }


    public static void DBCContextSettings<T>(this IServiceCollection services, IConfiguration configuration, string source)
        where T: DbContext
    {
       DbSettings dbSettings = new();
       configuration.GetSection(source).Bind(dbSettings);

        if (!dbSettings.IsValid)
            throw new ArgumentNullException($"Error :{nameof(DbSettings)} configuration is invalid.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbSettings.ConnectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<T>((options) =>
        {
            options.UseNpgsql(dbSettings.ConnectionString,
                o => o.UseNetTopologySuite());
            options.EnableSensitiveDataLogging();
        });      
    }


    public static void DBCContextPoolSettings<T>(this IServiceCollection services, IConfiguration configuration, string source)
        where T : DbContext
    {
        DbSettings dbSettings = new();
        configuration.GetSection(source).Bind(dbSettings);

        if (!dbSettings.IsValid)
            throw new ArgumentNullException($"Error :{nameof(DbSettings)} configuration is invalid.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbSettings.ConnectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContextPool<T>((serviceProvider,options) =>
        {            
            options.UseNpgsql(dbSettings.ConnectionString,
                o => o.UseNetTopologySuite());
            options.EnableSensitiveDataLogging();
        });
    }

    public static int PageTo(this int index, int take) => (index - 1) * take;
}


