using System;
using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork(StoreContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    //de wijzigingen opslaan in de database
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    //databaseconnectie sluiten
    public void Dispose()
    {
        context.Dispose();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        //de naam van de betreffende entiteit ophalen
        var type = typeof(TEntity).Name;

        //kijken of er al een repository voor dit type bestaat in de dictionary, anders wordt er een nieuwe aangemaakt
        return(IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, context) ?? throw new InvalidOperationException($"Probleem bij het creëren van een repository instantie voor {t}");
        });
    }
}
