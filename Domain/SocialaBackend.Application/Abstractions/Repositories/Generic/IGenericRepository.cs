using SocialaBackend.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Repositories.Generic
{
    public interface IGenericRepository<T> where T : BaseEntity, new()
    {
        Task CreateAsync(T entity);
        IQueryable<T> GetAll(
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes);

        IQueryable<T> OrderAndGet(
            Expression<Func<T, object>> order,
            bool isDescending,
            Expression<Func<T, bool>>? expression = null,
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes);

        IQueryable<T> SearchAndGet(
            Expression<Func<T, bool>> expression,
            int? skip = null,
            int? limit = null,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes);

        Task<T> GetByIdAsync(
            int id,
            bool isTracking = false,
            bool iqnoreQuery = false,
            Expression<Func<T, object>>? expressionIncludes = null,
            params string[] includes);
        Task<T> Get(
            Expression<Func<T, bool>> expression,
            bool isTracking = false,
            bool iqnoreQuery = false,
            params string[] includes);

        Task<ICollection<T>> GetCollection(Expression<Func<T, bool>> expression, int skip=0, int take=10, bool isTracking = false, bool iqnoreQuery = false, params string[] includes);
        void Update(T entity);
        void Delete(T entity);
        void SoftDelete(T entity);
        void RevertSoftDelete(T entity);
        Task SaveChangesAsync();
        Task<bool> IsExistEntityAsync(Expression<Func<T, bool>> expression);
        Task<T> GetEntityByIdWithSkipIncludes(int id, bool isTracking = false, bool iqnoreQuery = false,params  Expression<Func<T, object>>[] expressions);
    }
}
