using Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected RepositoryContext context;
        public RepositoryBase(RepositoryContext context)
        {
            this.context = context;
        }
        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ? context.Set<T>() : context.Set<T>().AsNoTracking(); 
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return trackChanges? context.Set<T>().Where(expression) : context.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity) => context.Set<T>().Add(entity);

        public void Delete(T entity) => context.Set<T>().Remove(entity);



        public void Update(T entity) => context.Set<T>().Update(entity);

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
