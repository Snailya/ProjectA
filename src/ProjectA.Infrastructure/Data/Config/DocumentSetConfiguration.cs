using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocSetAggregate;

namespace ProjectA.Infrastructure.Data.Config
{
    public class DocumentSetConfiguration : IEntityTypeConfiguration<DocumentSet>
    {
        public void Configure(EntityTypeBuilder<DocumentSet> builder)
        {
            builder.HasKey(x => x.Guid);
            builder.OwnsMany(
                p => p.Logs, a =>
                {
                    a.HasKey(x => x.Guid);

                    a.WithOwner().HasForeignKey("Guid");
                });
        }
    }
}