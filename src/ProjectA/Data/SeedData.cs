using ProjectA.Models;

namespace ProjectA
{
    public static class SeedData
    {
        public static void PopulateTestData(DocumentContext context)
        {
            context.Database.EnsureCreated();
            context.Database.EnsureCreated();
            
            foreach (var item in context.Documents) context.Remove(item);
            context.SaveChanges();

            var document1 = new Document(668407, 75696);
            context.Documents.Add(document1);
            context.SaveChanges();
        }
    }
}