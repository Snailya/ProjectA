using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocSetAggregate;
using ProjectA.Core.Shared;

namespace ProjectA.Infrastructure.Data
{
    public class DocumentContext : DbContext
    {
        private readonly IMediator _mediator;

        public DocumentContext(DbContextOptions<DocumentContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentSet> DocumentSets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder.UseSqlite("Data Source=document.db;Foreign Keys=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(x => x.Guid);
            modelBuilder.Entity<Document>().HasOne(p => p.LinkedDoc);
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // ignore events if no dispatcher provided
            if (_mediator == null) return result;

            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events) await _mediator.Publish(domainEvent).ConfigureAwait(false);
            }

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}