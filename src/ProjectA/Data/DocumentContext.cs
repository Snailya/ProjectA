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
    }
}