using DataLayer.Enums;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Context
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        #region Entities

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Warning> Warnings { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Apply default configuration first
            base.OnModelCreating(modelBuilder);

            //Prevent DateCreated from being updated
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.DateCreated)
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Friendship>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();



            //Check all entities for soft deletion
            modelBuilder.Entity<Post>().HasQueryFilter(p => p.DeletedAt == null);
            modelBuilder.Entity<Comment>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);
            modelBuilder.Entity<Friendship>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);
            modelBuilder.Entity<Ban>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);
            modelBuilder.Entity<Reaction>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);
            modelBuilder.Entity<Warning>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);
            modelBuilder.Entity<Attachment>().HasQueryFilter(p => p.DeletedAt == null && p.CreatedBy == DataCreation.User);

            #region Relationships

            modelBuilder.Entity<User>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Posts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.WarningsReceived)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.WarningsGiven)
                .WithOne(c => c.Moderator)
                .HasForeignKey(c => c.ModeratorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.BansReceived)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(p => p.BansGiven)
                .WithOne(c => c.Moderator)
                .HasForeignKey(c => c.ModeratorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany(u => u.FriendshipsOfRequester)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Recipient)
                .WithMany(u => u.FriendshipsOfRecipient)
                .HasForeignKey(f => f.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Post>()
            //    .HasOne(p => p.File)
            //    .WithOne(c => c.Post)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reaction>()
                .HasOne(f => f.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>()
                .HasOne(f => f.Comment)
                .WithMany(u => u.Reactions)
                .HasForeignKey(f => f.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>()
                .HasOne(f => f.User)
                .WithMany(u => u.Attachments)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion


            ////Composite key for Like entity
            //modelBuilder.Entity<Like>()
            //    .HasKey(l => new { l.CommentId, l.UserId });

            ////Composite key for Dislike entity
            //modelBuilder.Entity<Dislike>()
            //    .HasKey(l => new { l.CommentId, l.UserId });

            ////Composite key for Friendship entity
            //modelBuilder.Entity<Friendship>()
            //    .HasKey(f => new { f.UserId1, f.UserId2 });

            ////Composite key for Warning entity
            //modelBuilder.Entity<Warning>()
            //    .HasKey(f => new { f.UserId, f.ModeratorId});

            #region Constraints

            //Make sure that RequesterId can't be equal to RecipientId, so an user can't befriend himself
            modelBuilder.Entity<Friendship>()
                .HasCheckConstraint("CK_Friendship_RequesterId_And_RecipientId_CantBeEqual",
                    "RequesterId != RecipientId");

            //Make sure that UserId can't be equal to ModeratorId
            modelBuilder.Entity<Warning>()
                .HasCheckConstraint("CK_Warning_UserId_And_ModeratorId_CantBeEqual",
                    "UserId != ModeratorId");

            modelBuilder.Entity<Ban>()
                .HasCheckConstraint("CK_Ban_UserId_And_ModeratorId_CantBeEqual",
                    "UserId != ModeratorId");

            modelBuilder.Entity<Comment>()
                .HasIndex(e => new { e.PostId, e.UserId })
                .IsUnique();

            modelBuilder.Entity<Friendship>()
                .HasIndex(e => new { e.RequesterId, e.RecipientId })
                .IsUnique();

            modelBuilder.Entity<Warning>()
                .HasIndex(e => new { e.UserId, e.ModeratorId })
                .IsUnique();

            modelBuilder.Entity<Ban>()
                .HasIndex(e => new { e.UserId, e.ModeratorId })
                .IsUnique();

            modelBuilder.Entity<Reaction>()
                .HasIndex(e => new { e.UserId, e.CommentId })
                .IsUnique();

            #endregion
        }
    }
}
