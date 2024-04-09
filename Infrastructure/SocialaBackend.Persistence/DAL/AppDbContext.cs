using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        public DbSet<Chat> Chats { get; set; }
        public DbSet<VerifyRequest> VerifyRequests { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryItem> StoryItems { get; set; }
        public DbSet<StoryItemWatcher> StoryItemWatchers { get; set; }
        public DbSet<MessageMedia> MessageMedias { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AvatarLikeItem> AvatarLikeItems { get; set; }
        public DbSet<PostLikeItem> PostLikeItems { get; set; }
        public DbSet<CommentLikeItem> CommentLikeItems { get; set; }
        public DbSet<PostItem> PostItems { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyQueryFilters();
            builder.Entity<AppUser>()
               .HasOne(p => p.Story)  // HasOne() specifies the navigation property on the principal side (Person).
               .WithOne(s => s.Owner)    // WithOne() specifies the navigation property on the dependent side (Passport).
               .HasForeignKey<Story>(p => p.OwnerId)  // HasForeignKey<TDependent>() specifies the foreign key property in the dependent entity (Passport).
               .IsRequired();
            builder.Entity<PostLikeItem>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<PostLikeItem>()
               .HasOne(p => p.LikedUser)
               .WithMany(u => u.LikedPosts)
               .HasForeignKey(p => p.LikedUserId)
               .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Chat>()
               .HasOne(p => p.SecondUser)
               .WithMany(u => u.Chats)
               .HasForeignKey(p => p.SecondUserId)
               .OnDelete(DeleteBehavior.NoAction);
            // Каскадное удаление

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
