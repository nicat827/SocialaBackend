using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories.Generic;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Persistence.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Repositories.Generic
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _table;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _table.AddAsync(entity);
        }
        public IQueryable<T> GetAll(
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();

            if (skip != null) query = query.Skip((int)skip);
            if (limit != null) query = query.Take((int)limit);

            if (includes != null) query = _takeIncludes(query, includes);

            return isTracking ? query : query.AsNoTracking();
        }
        public IQueryable<T> SearchAndGet(
            Expression<Func<T, bool>> expression,
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(expression);
            if (skip != null) query = query.Skip((int)skip);
            if (limit != null) query = query.Take((int)limit);

            if (includes != null) query = _takeIncludes(query, includes);
            return query;
        }



        public IQueryable<T> OrderAndGet(
            Expression<Func<T, object>> order,
            bool isDescending,
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();

            if (!isDescending) query = query.OrderBy(order);
            else query = query.OrderByDescending(order);

            if (skip != null) query = query.Skip((int)skip);
            if (limit != null) query = query.Take((int)limit);

            if (includes != null) query = _takeIncludes(query, includes);

            return isTracking ? query : query.AsNoTracking();
        }
        public async Task<T> GetByIdAsync(int id, bool isTracking = false, bool iqnoreQuery = false, Expression<Func<T, object>>? expression = null, params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(e => e.Id == id);
            if (expression is not null)
            {
                query = query.Include(expression);
            }
            if (includes != null)
            {
                query = _takeIncludes(query, includes);
            }
            return isTracking ? await query.FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<T> Get(Expression<Func<T, bool>> expression, bool isTracking = false, bool iqnoreQuery = false, params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(expression);
            if (includes != null) query = _takeIncludes(query, includes);

            return isTracking ? await query.FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<ICollection<T>> GetCollection(Expression<Func<T, bool>> expression,int skip = 0, int take = 10, bool isTracking = false, bool iqnoreQuery = false, params string[] includes)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(expression);
            if (skip> 0) query = query.Skip(skip);
            if (take > 0) query = query.Take(take);
            if (includes != null) query = _takeIncludes(query, includes);

            return isTracking ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();
        }
        public void Update(T entity)
        {
            _table.Update(entity);
        }
        public void SoftDelete(T entity)
        {
            entity.IsDeleted = true;
        }
        public void RevertSoftDelete(T entity)
        {
            entity.IsDeleted = false;
        }
        public void Delete(T entity)
        {
            _table.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsExistEntityAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _table;
            query = query.IgnoreQueryFilters();
            return await query.AnyAsync(expression);
        }

        public async Task<T> GetEntityByIdWithSkipIncludes(int id, bool isTracking = false, bool iqnoreQuery = false, params Expression<Func<T, object>>[] expressions)
        {
            IQueryable<T> query = _table;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(e => e.Id == id);
            foreach (var expr  in expressions)
            {
                query = query.Include(expr);
            }
            return isTracking ? await query.FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }


        private IQueryable<T> _takeIncludes(IQueryable<T> query, params string[] includes)
        {
            for (int i = 0; i < includes.Length; i++)
            {
                query = query.Include(includes[i]);
            }

            return query;
        }

    }
}
