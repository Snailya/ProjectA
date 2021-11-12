using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Infrastructure.Data;

namespace ProjectA.Test
{
    [TestFixture]
    public class DatabaseFixture
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite(CreateInMemoryDatabase()));

            return services;
        }

        protected void Build(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();

            var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();
            dbContext.Database.EnsureCreated();
        }

        protected ServiceProvider ServiceProvider;

        protected void AppendTestDataToDatabase(Document[] documents)
        {
            var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();
            dbContext.Documents.AddRange(documents);
            dbContext.SaveChanges();
        }

        protected void ClearTestData()
        {
            var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();
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