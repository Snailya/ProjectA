using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Data;
using ProjectA.Models;

namespace ProjectA.Test
{
    [TestFixture]
    public class DatabaseFixture
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DocumentContext>(options =>
                options.UseSqlite(CreateInMemoryDatabase()));

            return services;
        }

        protected void Build(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            dbContext.Database.EnsureCreated();
            dbContext.Database.EnsureCreated();
        }

        protected ServiceProvider ServiceProvider;
        
        protected void AppendTestDataToDatabase(Document[] documents)
        {
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            dbContext.Documents.AddRange(documents);
            dbContext.SaveChanges();
        }

        protected void ClearTestData()
        {
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            dbContext.Documents.RemoveRange(dbContext.Documents);
            dbContext.SaveChanges();
        }
        
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
    }
}