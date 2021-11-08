using Microsoft.EntityFrameworkCore;
using ProjectA.Core.Models;

namespace ProjectA.Infrastructure.Data
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

            optionsBuilder.UseSqlite("Data Source=document.db;Foreign Keys=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(x => x.Guid);
            modelBuilder.Entity<Document>().HasOne(p => p.Snapshot);
            modelBuilder.Entity<Document>().OwnsMany(
                p => p.Versions, a =>
                {
                    a.HasKey(x => x.Guid);

                    a.WithOwner().HasForeignKey("EntityId");
                    a.OwnsOne(dv => dv.VersionNumber);

                    // a.Property<Guid>("Id");
                    // a.HasKey("Id");
                });
        }
    }
}