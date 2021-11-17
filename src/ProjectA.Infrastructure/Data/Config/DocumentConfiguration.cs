using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Infrastructure.Data.Config
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.Guid);
            builder.HasMany(p => p.Bindings);
            builder.OwnsMany(
                p => p.Versions, a =>
                {
                    a.HasKey(p => p.Guid);

                    a.WithOwner().HasForeignKey("EntityId");
                    a.OwnsOne(dv => dv.VersionNumber);
                });
        }
    }
}