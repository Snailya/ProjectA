using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectA
{
    public class DocumentContextFactory : IDesignTimeDbContextFactory<DocumentContext>
    {
        public DocumentContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DocumentContext>();
            optionsBuilder.UseSqlite("Data Source=document.db");

            return new DocumentContext(optionsBuilder.Options);
        }
    }
}