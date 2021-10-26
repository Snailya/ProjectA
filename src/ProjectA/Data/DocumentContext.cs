using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProjectA.Models;

namespace ProjectA
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder.UseSqlite("Data Source=document.db");
            optionsBuilder.LogTo(s => Debug.WriteLine(s));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().OwnsMany(
                p => p.Versions, a =>
                {
                    a.WithOwner().HasForeignKey("EntityId");

                    a.OwnsOne(dv => dv.VersionNumber);
                    // a.HasKey("VersionId");

                    a.Property<Guid>("Id");
                    a.HasKey("Id");
                }).HasKey(p => p.EntityId);
        }
    }
}