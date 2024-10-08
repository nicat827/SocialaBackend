using Microsoft.EntityFrameworkCore;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Common
{
    internal static  class QueryFilterHandler
    {
        public static void ApplyFilter<T>(this ModelBuilder builder) where T : BaseEntity, new()
        {
            builder.Entity<T>().HasQueryFilter(x => x.IsDeleted == false);
        }
      

        public static void ApplyQueryFilters(this ModelBuilder builder)
        {
            builder.ApplyFilter<Comment>();
            builder.ApplyFilter<Post>();
            builder.ApplyFilter<PostLikeItem>();
            builder.ApplyFilter<CommentLikeItem>();
        }
    }
}
