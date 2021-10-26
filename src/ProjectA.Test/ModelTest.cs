using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Models;

namespace ProjectA.Test
{
    public class ModelTest
    {
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            //setup our DI
            _serviceProvider = new ServiceCollection()
                .AddDbContext<DocumentContext>(options => options.UseSqlite("Data Source=document.db;"))
                .BuildServiceProvider();

            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            dbContext.Database.EnsureCreated();
            dbContext.Database.EnsureCreated();
        }

        [Test]
        public void AddDocument()
        {
            var entityId = new Random().Next();
            var expected = new Document(entityId);

            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            dbContext.Add(expected);
            dbContext.SaveChanges();

            var actual = dbContext.Documents.Find(entityId);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddDocumentVersion()
        {
            var versionId = new Random().Next();
            var expected = new DocVersion {VersionId = versionId, VersionNumber = new VersionNumber(1, 0)};

            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            var document = dbContext.Documents.First();
            document.UpdateVersion(expected);
            dbContext.SaveChanges();

            var actual = dbContext.Documents.Find(document.EntityId).Versions.Single(x => x == expected);
            Assert.AreEqual(expected, actual);
        }
    }
}