using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Persistence.Common;

namespace SocialaBackend.Persistence.DAL
{
    internal class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyQueryFilters();

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in entities)
            {
                switch (data.State)
                {
                    case EntityState.Modified:
                        data.Entity.LastUpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        data.Entity.CreatedAt = DateTime.UtcNow;
                        data.Entity.CreatedBy = "Admin";
                        break;

                }
            }
            return base.SaveChanges();
        }
    }
}
