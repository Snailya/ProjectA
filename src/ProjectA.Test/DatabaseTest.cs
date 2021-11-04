using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Core.Data;
using ProjectA.Core.Models;

namespace ProjectA.Test
{
    public class DatabaseTest : DatabaseFixture
    {
        [OneTimeSetUp]
        public void Init()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            // build service provider
            Build(services);
        }


        [Test]
        public void SaveChangesSuccessfully_WhenAddDocument()
        {
            // arrange
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            var document = new Document(new Random().Next());

            // act
            dbContext.Documents.Add(document);
            dbContext.SaveChanges();

            // assert
            var actual = dbContext.Documents.SingleOrDefault(x => x.EntityId == document.EntityId);
            Assert.NotNull(actual);
        }

        [Test]
        public void SaveChangesSuccessfully_WhenUpdateDocumentVersion()
        {
            // arrange
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());

            var version = new DocVersion {VersionId = new Random().Next(), VersionNumber = new VersionNumber(1, 0)};

            // act
            var document = dbContext.Documents.First();
            document.UpdateVersion(version);
            dbContext.SaveChanges();

            // assert
            var actual = dbContext.Documents.Single(x => x.EntityId == document.EntityId).Versions
                .SingleOrDefault(x => x == version);
            Assert.NotNull(actual);
        }

        [Test]
        public void SaveChangesSuccessfully_WhenUpdateSnapshot()
        {
            // arrange
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());

            var snapshot = new Document(new Random().Next());

            // act
            var document = dbContext.Documents.First();
            document.UpdateSnapshot(snapshot);
            dbContext.SaveChanges();

            // assert
            Assert.NotNull(document.Snapshot);
        }

        [Test]
        public void SaveChangesSuccessfully_WhenSetSnapshotFolder()
        {
            // arrange
            using var dbContext = new DocumentContext(
                ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            var expected = new Random().Next();

            // act
            var document = dbContext.Documents.First(x => x.Snapshot == null);
            document.SetSnapshotFolder(expected);
            dbContext.SaveChanges();

            var actual = document.SnapshotFolderId;

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}