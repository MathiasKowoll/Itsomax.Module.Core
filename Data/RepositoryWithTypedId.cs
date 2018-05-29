using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Data.Infrastructure.Models;
using System.Threading.Tasks;

namespace Itsomax.Module.Core.Data
{
    public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
    {
        //public RepositoryWithTypedId(ItsomaxDbContext context)
        //{
        //    Context = context;
            //DbSet = Context.Set<T>();
        //}

        protected readonly DbContext Context;
        //protected DbSet<T> DbSet { get; }

        public RepositoryWithTypedId(DbContext context)
        {
            Context = context;
            //DbSet = Context.Set<T>()
        }
        
        public T Get(long id)
        {
            return Context.Set<T>().Find(id);
        }
        
        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }
        
        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
        
        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        /*
        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }
        
        public IQueryable<T> Query()
        {
            return DbSet;
        }
        */
        
        

        /*
        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
        */
    }
}