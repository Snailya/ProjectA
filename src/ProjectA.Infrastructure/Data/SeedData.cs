using System.Linq;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Infrastructure.Data
{
    public static class SeedData
    {
        public static void PopulateTestData(AppDbContext context)
        {
            context.Database.EnsureCreated();
            context.Database.EnsureCreated();

            if (context.Documents.Any())
            {
                foreach (var item in context.Documents) context.Remove(item);
                context.SaveChanges();
            }

            var document1 = new Document(668407);
            document1.UpdateVersion(new DocumentVersion
                {VersionId = 810828, VersionNumber = new DocumentVersionNumber( 1, 0)});
            context.Documents.Add(document1);
            context.SaveChanges();
        }
    }
}