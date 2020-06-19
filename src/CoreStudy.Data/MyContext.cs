using CoreStudy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreStudy.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var post = builder.Entity<Post>();
            post.HasKey(c => new { c.Id });
            post.HasIndex(c => new { c.CategoryId });
            post.Property(p => p.Title).HasMaxLength(20);

            var category = builder.Entity<Category>();
            category.HasKey(c => new { c.Id });
            category.HasIndex(c => new { c.DisplayName });
            category.Property(p => p.DisplayName).HasMaxLength(40);

            base.OnModelCreating(builder);
        }
    }
}
