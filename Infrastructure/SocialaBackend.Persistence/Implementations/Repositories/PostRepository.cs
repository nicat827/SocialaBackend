using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Persistence.DAL;
using SocialaBackend.Persistence.Implementations.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Repositories
{
    internal class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly DbSet<Post> _posts;
        public PostRepository(AppDbContext context) : base(context)
        {
            _posts = context.Set<Post>();
        }

        public async Task<Post> GetPostByIdWithExpersionIncludes(int id, Expression<Func<Post,IEnumerable<BaseEntity>>> expression, bool isTracking = false, bool iqnoreQuery = false)
        {
            IQueryable<Post> query = _posts;
            if (iqnoreQuery) query = query.IgnoreQueryFilters();
            query = query.Where(e => e.Id == id);
            query = query.Include(expression);
            return isTracking ? await query.FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
