using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Data.Infrastructure.Models;
using System.Threading.Tasks;

namespace Itsomax.Module.Core.Data
{
    public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
    {
        public RepositoryWithTypedId(ItsomaxDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        protected DbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public IQueryable<T> Query()
        {
            return DbSet;
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}