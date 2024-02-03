using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Persistence.Common;

namespace SocialaBackend.Persistence.DAL
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<PostLikeItem> PostLikeItems { get; set; }
        public DbSet<CommentLikeItem> CommentLikeItems { get; set; }
        public DbSet<PostItem> PostItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyQueryFilters();
            
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in entities)
            {
                switch (data.State)
                {
                    case EntityState.Added:
                        data.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
