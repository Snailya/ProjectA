using System;
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
        protected IServiceCollection ConfigureServices()
        {
            return new ServiceCollection().AddDbContext<DocumentContext>(options =>
                options.UseSqlite(CreateInMemoryDatabase()));
        }

        protected virtual void PopulateTestData(DocumentContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.EnsureCreated();

            var document = new Document(new Random().Next());
            dbContext.Add(document);
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