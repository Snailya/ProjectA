using System.Linq;
using ProjectA.Core.Models;

namespace ProjectA.Infrastructure.Data
{
    public static class SeedData
    {
        public static void PopulateTestData(DocumentContext context)
        {
            context.Database.EnsureCreated();
            context.Database.EnsureCreated();

            if (context.Documents.Any())
            {
                foreach (var item in context.Documents) context.Remove(item);
                context.SaveChanges();
            }

            var document1 = new Document(668407, 75696);
            document1.UpdateVersion(new DocVersion {VersionId = 810828, VersionNumber = new VersionNumber(1, 0)});
            context.Documents.Add(document1);
            context.SaveChanges();
        }
    }
}