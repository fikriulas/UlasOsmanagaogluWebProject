using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UlasBlog.Entity;

namespace UlasBlog.Data.Concrete.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogCategory>()
                .HasKey(pk => new { pk.BlogId, pk.CategoryId }); // tablonun iki tane primary key'i var.
            modelBuilder.Entity<Blog>()
                .HasIndex(u => u.SlugUrl)
                .IsUnique(true);
            modelBuilder.Entity<Category>()
                .HasIndex(u => u.SlugUrl)
                .IsUnique(true);
        }

    }

}
